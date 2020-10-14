using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

namespace Blazor.Startechmanager.Server.Controllers
{
    [Serializable]
    public class UserControllerException : Exception
    {
        public UserControllerException() { }
        public UserControllerException(string message) : base(message) { }
        public UserControllerException(string message, Exception inner) : base(message, inner) { }
        protected UserControllerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Route("{controller}/{action}/{userId:int}")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly  UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthorizationService authorizationService;

        public UserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authorizationService = authorizationService;
        }

        public async Task<UserObject> GetUser(int userId)
        {
            ApplicationUser user;
            if(userId == ThisUser.Id)
            {
                user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

                if (user is null)
                {
                    throw new UserControllerException("Unknow user");
                }
            }
            else
            {
                user =  await dbContext.Users.Include(x => x.Startechs).FirstOrDefaultAsync(x => x.Id == userId);

                if(!user?.Startechs.Any( x =>  CallerIsLeaderOf(x.Startech)) ?? false)
                {
                    throw new UserControllerException("you don't have the right to view this");
                }
                
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new UserObject { Id = user.Id, UserName = user.UserName, NumberOfpoints = user.NumberOfPoints };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private bool CallerIsLeaderOf(Startechs startech)
        {
            return authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, StartechPolicyHelper.GetPolicyName(startech, MustBeLeader: true)).GetAwaiter().GetResult().Succeeded;
        }
    }
}
