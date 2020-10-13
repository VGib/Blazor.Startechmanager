using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Startechmanager.Server.Controllers
{
    public class AuthorizeAdminMember : IAsyncAuthorizationFilter
    {
#nullable disable
        private readonly IAuthorizationService AuthorizationService;
#nullable enable
        public AuthorizeAdminMember(IAuthorizationService authorizationService)
        {
            AuthorizationService = authorizationService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var startechTypeRouteValue = context.RouteData.Values["startechType"] as string;
            if (startechTypeRouteValue is null)
            {
                context.Result = new UnauthorizedResult();
            }
            if (!Enum.TryParse(typeof(Startechs), startechTypeRouteValue, out var startechType)
                || !(await AuthorizationService.AuthorizeAsync(context.HttpContext.User, StartechPolicyHelper.GetPolicyName((Startechs) startechType, MustBeLeader: true))).Succeeded
                )
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

    [ServiceFilter(typeof(AuthorizeAdminMember), IsReusable = false)]
    [Route("{controller}/{startechType}/{action}/{userId:int?}")]
    public class AdminMemberController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminMemberController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<UserObject>> GetMembers(Startechs startechType)
        {
            return await dbContext.Users.Where(x => x.Startechs.Any(y => y.Startech == startechType && !y.IsLeader && x.Id == y.ApplicationUserId))
                .Select(x => new UserObject { Id = x.Id, UserName = x.UserName, NumberOfpoints = x.NumberOfPoints}).ToListAsync();
        }

        public async Task<IActionResult> SetMember(Startechs startechType, int userId)
        {
            if (!await dbContext.MappingStartechs.AnyAsync(x => x.ApplicationUserId == userId && x.Startech == startechType))
            {
                dbContext.Add(new MappingStartechUser
                {
                    ApplicationUserId = userId,
                    Startech = startechType,
                    IsLeader = false
                });
                await dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        public async Task<IActionResult> RemoveMember(Startechs startechType, int userId)
        {
            var startechMembership = await dbContext.MappingStartechs.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.Startech == startechType);
            if(startechMembership is null)
            {
                return BadRequest($"the user is not member of startech {startechType}");
            }
            if(startechMembership.IsLeader)
            {
                return BadRequest("only admin in an admin section can remove a leader");
            }

            dbContext.Remove(startechMembership);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
