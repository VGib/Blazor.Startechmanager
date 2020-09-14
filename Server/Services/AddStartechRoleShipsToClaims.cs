using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Services
{
    public class ApplicationUserClaimFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly ApplicationDbContext dbContext;

        public ApplicationUserClaimFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options, ApplicationDbContext dbContext) : base(userManager, roleManager, options)
        {
            this.dbContext = dbContext;
        }

        protected async override Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claims =  await base.GenerateClaimsAsync(user);
            var startechs = dbContext.MappingStartechs.Where(x => x.ApplicationUserId == user.Id);
            claims.AddClaims(startechs.Select(x => new Claim( StartechClaimHelper.GetStartechClaimType( x.Startech ), x.IsLeader ? StartechClaimHelper.Leader :  StartechClaimHelper.Member)));
            return claims;
        }
    }
}
