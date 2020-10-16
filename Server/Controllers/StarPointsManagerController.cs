using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Constants;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Controllers
{
    [Serializable]
    public class StarpointManagerException : Exception
    {
        public StarpointManagerException() { }
        public StarpointManagerException(string message) : base(message) { }
        public StarpointManagerException(string message, Exception inner) : base(message, inner) { }
        protected StarpointManagerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Route("{controller}/{action}/{userId:int?}")]
    public class StarPointsManagerController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthorizationService authorizationService;

        public StarPointsManagerController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authorizationService = authorizationService;
        }

        public async Task<IList<StarpointsType>> GetStarpointsType()
        {
            return await dbContext.StarpointsType.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<IList<StarpointsItem>> GetStarpoints([FromRoute] int userId, [FromQuery] TimeSpan? history)
        {
            var historyWithDefault = DateTime.Now.Add(-history ?? TimeSpan.FromDays(-730));
            (var userToDealWith, var startechs, var isLeader) = await GetStartechsToStudyForUser(userId);

            if (!isLeader)
            {
                startechs = GetAllStartechs();
            }

            return await dbContext.StarpointsItem.Where(x => x.ApplicationUserId == userToDealWith.Id && startechs.Contains(x.Startech))
                        .Where(x => x.Date > historyWithDefault).ToListAsync();
        }

        public async Task<IList<StarpointsItem>> GetInValidationStarpoints()
        {
            var startechs = FromUserToHisStartechs(await GetThisUser(returnOnlyStartechWhereUserIsLeader: true));
            return await dbContext.StarpointsItem.Where(x => startechs.Contains(x.Startech) && x.ValidationState == ValidationState.InStudy).ToListAsync();
        }

        public async Task<IActionResult> CreateStarpoints([FromRoute] int userId, [FromBody] StarpointsItem itemToCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"your model is not valid :: {ModelState.GetNonValidationErrorMessage()}");
            }

            (var userToDealWith, var startechs, var isLeader) = await GetStartechsToStudyForUser(userId);

            if (!startechs.Contains(itemToCreate.Startech))
            {
                return StatusCode((int)HttpStatusCode.MethodNotAllowed, $"you or user don't have enought right on startech {itemToCreate.Startech}");
            }

            itemToCreate.ApplicationUserId = userToDealWith.Id;

#pragma warning disable CS8604 // Possible null reference argument.
            StarpointsType? starpointTypeToCreate = await GetStarpointType(itemToCreate.Type);
#pragma warning restore CS8604 // Possible null reference argument.
            itemToCreate.StarpointsTypeId = starpointTypeToCreate?.Id;
            itemToCreate.ValidationState = isLeader ? ValidationState.Validated : ValidationState.InStudy;
            itemToCreate.Date = DateTime.Now;

            if (!isLeader)
            {
                if (starpointTypeToCreate is null)
                {
                    return BadRequest("only startech leader can create starpoints from non typed startech");
                }
                itemToCreate.NumberOfPoints = starpointTypeToCreate.NumberOfPoint;
            }
            else
            {
                if (itemToCreate.Type != null && starpointTypeToCreate is null)
                {
                    return BadRequest($"the type {itemToCreate.Type.TypeName} don't exist");
                }

                userToDealWith.NumberOfPoints += itemToCreate.NumberOfPoints;
            }

            dbContext.Add(itemToCreate);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("{itemToUpdateId:int}/{newStatus}")]
        public async Task<IActionResult> UpdateValidationStatus([FromRoute] int itemToUpdateId, [FromRoute] ValidationState newStatus)
        {
            StarpointsItem? itemToUpdate = await GetThisItem(itemToUpdateId);

            if (itemToUpdate is null)
            {
                return BadRequest($"unknown starpoint item {itemToUpdateId}");
            }

            if (!IsStartechLeader(itemToUpdate.Startech))
            {
                return BadRequest($"you should be leader of startech {itemToUpdate.Startech}");
            }

            if (itemToUpdate.ValidationState == newStatus)
            {
                return BadRequest("you're trying to modify the same  status");
            }

            if (itemToUpdate.ValidationState != ValidationState.Validated && newStatus == ValidationState.Validated)
            {
                await AddPointToUser(itemToUpdate.ApplicationUserId, itemToUpdate.NumberOfPoints);
            }
            else if (itemToUpdate.ValidationState == ValidationState.Validated)
            {
                await AddPointToUser(itemToUpdate.ApplicationUserId, -itemToUpdate.NumberOfPoints);
            }

            itemToUpdate.ValidationState = newStatus;
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("{itemId:int}")]
        public async Task<StarpointsItem> GetItem(int itemId)
        {
            var item = await GetThisItem(itemId);
            var thisUser = await GetThisUser(returnOnlyStartechWhereUserIsLeader: true);

            if(item.ApplicationUserId != thisUser.Id && !thisUser.Startechs.Any(x => x.Startech == item.Startech))
            {
                throw new StarpointManagerException($"you don't have right to view item {itemId}");
            }

            return item;
        }

        private async Task<StarpointsItem> GetThisItem(int itemId)
        {
            return await dbContext.StarpointsItem.FirstOrDefaultAsync(x => x.Id == itemId);
        }

        public async Task<IActionResult> UpdateStarpoints([FromBody] StarpointsItem starpointToUpdate)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest($"starpoint item is invalid: {ModelState.GetNonValidationErrorMessage()}");
            }

            var inDatatabaseStarpointToUpdate =  await GetThisItem(starpointToUpdate.Id);

            if(inDatatabaseStarpointToUpdate is null)
            {
                return BadRequest($"unknow starpoint item {starpointToUpdate.Id}");
            }

            var thisUser = await GetThisUser(returnOnlyStartechWhereUserIsLeader: true);

            bool isStartechLeader = thisUser.Id != inDatatabaseStarpointToUpdate.ApplicationUserId;
            if (isStartechLeader && !IsStartechLeader(inDatatabaseStarpointToUpdate.Startech))
            {
                return BadRequest("you don't have the right to do this");
            }
            else if (!isStartechLeader && inDatatabaseStarpointToUpdate.ValidationState != ValidationState.InStudy)
            {
                return BadRequest("current user can only modify InStudy state starpoints item");
            }
           

            if(isStartechLeader)
            {
                if(inDatatabaseStarpointToUpdate.NumberOfPoints != starpointToUpdate.NumberOfPoints && inDatatabaseStarpointToUpdate.ValidationState == ValidationState.Validated)
                {
                    var userOfUpdatedStarpointsItem = await GetUser(inDatatabaseStarpointToUpdate.ApplicationUserId);
                    userOfUpdatedStarpointsItem.NumberOfPoints += starpointToUpdate.NumberOfPoints - inDatatabaseStarpointToUpdate.NumberOfPoints;
                    inDatatabaseStarpointToUpdate.NumberOfPoints = starpointToUpdate.NumberOfPoints;
                }

                var starpointType = await GetStarpointType(starpointToUpdate.Type);
                if(starpointType?.Id != inDatatabaseStarpointToUpdate.StarpointsTypeId)
                {
                    inDatatabaseStarpointToUpdate.StarpointsTypeId = starpointType?.Id;
                }
            }

            // update only what is necessary
            if (inDatatabaseStarpointToUpdate.TextJustification != starpointToUpdate.TextJustification)
            {
                inDatatabaseStarpointToUpdate.TextJustification = starpointToUpdate.TextJustification;
            }

            if (inDatatabaseStarpointToUpdate.UrlJustification != starpointToUpdate.UrlJustification)
            {
                inDatatabaseStarpointToUpdate.UrlJustification = starpointToUpdate.UrlJustification;
            }

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        private Task<ApplicationUser> GetUser(int applicationUserId)
        {
            return dbContext.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
        }

        [Route("{itemToCancelId:int}")]
        public async Task<IActionResult> CancelStarpoints(int itemToCancelId)
        {
            var itemToCancel = await GetThisItem(itemToCancelId);

            if(itemToCancel is null)
            {
                return BadRequest($"unknown starpoints item id {itemToCancelId}");
            }

            if(itemToCancel.ValidationState != ValidationState.InStudy)
            {
                return BadRequest("item to cancel should be in study state");
            }

            var user = await GetThisUser(returnOnlyStartechWhereUserIsLeader: true);

            if(itemToCancel.ApplicationUserId != user.Id &&  !user.Startechs.Any(x => x.Startech == itemToCancel.Startech))
            {
                return BadRequest($"you don't have enougth right to cancel item {itemToCancelId}");
            }

            dbContext.Remove(itemToCancel);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private  async Task AddPointToUser(int applicationUserId, int numberOfPoints)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
            if(user != null)
            {
                user.NumberOfPoints += numberOfPoints;
            }
        }

        private async Task<StarpointsType> GetStarpointType(StarpointsType? type)
        {
            var typeToCreateId = type?.Id ?? -1;
            return  await dbContext.StarpointsType.FirstOrDefaultAsync(x => x.Id == typeToCreateId && x.IsActive);
        }

        private async Task<ApplicationUser> GetThisUser( bool returnOnlyStartechWhereUserIsLeader = false)
        {
            Expression<Func<MappingStartechUser, bool>> filterStartechWithIsLeader = GetIsLeaderFilter(returnOnlyStartechWhereUserIsLeader);
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            user.Startechs = await dbContext.MappingStartechs.Where(x => x.ApplicationUserId == user.Id).Where(filterStartechWithIsLeader).ToListAsync();
            return user;
        }

        private static Expression<Func<MappingStartechUser, bool>> GetIsLeaderFilter(bool returnOnlyStartechWhereUserIsLeader)
        {
            if(returnOnlyStartechWhereUserIsLeader)
            {
                return x => x.IsLeader;
            }
            else
            {
                return _ => true;
            }
        }

        private static IList<Startechs> GetAllStartechs()
        {
            return Enum.GetValues(typeof(Startechs)).Cast<Startechs>().ToArray();
        }
        private  async Task<(ApplicationUser , IList<Startechs> startechs, bool isLeader)> GetStartechsToStudyForUser(int userId)
        {
            if(userId == ThisUser.Id)
            {
                var user = await GetThisUser(returnOnlyStartechWhereUserIsLeader: false);

                return (user, FromUserToHisStartechs(user), false);
            }
            else
            {
                if(!(await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, StartechPolicyHelper.AllStartechLeader)).Succeeded)
                {
                    throw new UnauthorizedAccessException("you should be a startech leader!");
                }

                var user = await dbContext.Users.Include(x => x.Startechs).FirstOrDefaultAsync(x => x.Id == userId);

                if(user is null)
                {
                    throw new StarpointManagerException("user not found");
                }

                return (user, user.Startechs.Select(x => x.Startech).Where(x => IsStartechLeader(x)).ToArray(), true);
            }
        }

        private static Startechs[] FromUserToHisStartechs(ApplicationUser user)
        {
            return user.Startechs.Select(x => x.Startech).ToArray();
        }

        private bool IsStartechLeader(Startechs x)
        {
            return authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, StartechPolicyHelper.GetPolicyName(x, MustBeLeader: true)).GetAwaiter().GetResult().Succeeded;
        }
    }
}
