using System;
using System.Threading.Tasks;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
    [Route("{controller}/{startechType}/{action}/{userd:int?}")]
    public class AdminMemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
