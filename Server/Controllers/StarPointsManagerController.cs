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

        public async Task<IList<StarpointsItem>> GetStarpoints( [FromRoute] int userId ,[FromQuery] TimeSpan? history )
        {
            var historyWithDefault = DateTime.Now.Add(-history ?? TimeSpan.FromDays(-730));
            ( var userIdToDealWith, var startechs, var isLeader) = await GetStartechsToStudyForUser(userId);

            if(!isLeader)
            {
                startechs = GetAllStartechs();
            }

            return await dbContext.StarpointsItem.Where(x => x.ApplicationUserId == userIdToDealWith && startechs.Contains(x.Startech))
                        .Where(x => x.Date > historyWithDefault).ToListAsync();
        }

        public async Task<IList<StarpointsItem>> GetInValidationStarpoints()
        {
            var startechs = FromUserToHisStartechs(await GetThisUser(returnOnlyStartechWhereUserIsLeader: true));
            return await dbContext.StarpointsItem.Where(x => startechs.Contains(x.Startech) && x.ValidationState == ValidationState.InStudy).ToListAsync();
        }

        public async Task<IActionResult> CreateStarpoints([FromRoute] int userId, [FromBody] StarpointsItem itemToCreate )
        {
            if(!ModelState.IsValid)
            {
               return BadRequest($"your model is not valid :: {ModelState.GetNonValidationErrorMessage()}");
            }

            (var userToDealWithId, var startechs, var isLeader) = await GetStartechsToStudyForUser(userId);

            if(!startechs.Contains(itemToCreate.Startech))
            {
                return StatusCode((int)HttpStatusCode.MethodNotAllowed, $"you or user don't have enought right on startech {itemToCreate.Startech}");
            }

            itemToCreate.ApplicationUserId = userToDealWithId;

            StarpointsType? starpointTypeToCreate = await GetStarpointType(itemToCreate.Type);
            itemToCreate.StarpointsTypeId = starpointTypeToCreate?.Id;
            itemToCreate.ValidationState = isLeader ? ValidationState.Validated : ValidationState.InStudy;

            if (!isLeader)
            {
                if (starpointTypeToCreate is null)
                {
                    return BadRequest("only startech leader can create starpoints from non typed startech");
                }
                itemToCreate.NumberOfPoints = starpointTypeToCreate.NumberOfPoint;
            }

            dbContext.Add(itemToCreate);
            dbContext.SaveChanges();

            return Ok();
        }

        private async Task<StarpointsType> GetStarpointType(StarpointsType type)
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
        private  async Task<(int userToDealWith , IList<Startechs> startechs, bool isLeader)> GetStartechsToStudyForUser(int userId)
        {
            if(userId == ThisUser.Id)
            {
                var user = await GetThisUser(returnOnlyStartechWhereUserIsLeader: false);

                return (user.Id, FromUserToHisStartechs(user), false);
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

                return (user.Id, user.Startechs.Select(x => x.Startech).Where(x => IsStartechLeader(x)).ToArray(), true);
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
