using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Startechmanager.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Startechmanager.Server.Controllers
{
    [Route("[controller]/{startechType}")]
    [ApiController]
    public class StartechOwnerController : ControllerBase
    {
        public string? StartechType { get; private set; }

        public ApplicationUser? User { get; private set; }

        public StartechOwnerController(UserManager<ApplicationUser> userManager) : base()
        {
            StartechType = RouteData.Values["startechType"] as string;
            var getUserTask = userManager.GetUserAsync(HttpContext.User);
            getUserTask.Wait();
            User = getUserTask.Result;
        }
    }
}
