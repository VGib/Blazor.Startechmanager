using Blazor.Startechmanager.Server.Services;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using NUnit.Framework;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class SendClaimsToBlazorClientTests
    {
        public IProfileService Target { get; set; } = new SendClaimsToBlazorClientProfileService();

        [Test]
        public async Task startech_claims_should_be_send_to_blazor_clients()
        {
            var claimToTransfert = new Claim(StartechClaimHelper.GetStartechClaimType(Startechs.Dotnet), StartechClaimHelper.Member);
            var profileRequest = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { claimToTransfert }))
            };
            await Target.GetProfileDataAsync(profileRequest);
            profileRequest.IssuedClaims.Select(x => x.Type).Should().BeEquivalentTo(claimToTransfert.Type);
            profileRequest.IssuedClaims.Select(x => x.Value).Should().BeEquivalentTo(claimToTransfert.Value);
        }
    }
}
