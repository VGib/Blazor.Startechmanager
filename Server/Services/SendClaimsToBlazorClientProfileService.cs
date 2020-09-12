using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityModel;
using System.Linq;

namespace Blazor.Startechmanager.Server.Services
{
    // Problem for remoting role in Blazor
    // https://github.com/dotnet/AspNetCore.Docs/issues/17649
    public class SendClaimsToBlazorClientProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);
            context.IssuedClaims.AddRange(roleClaims);
            var startechClaims = context.Subject.Claims.Where(x => x.Type.StartsWith("StartechMember::"));
            context.IssuedClaims.AddRange(startechClaims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
