using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class AuthorizationTests
    {
        public IAuthorizationService AuthorizationService { get; set; }

        public ClaimsPrincipal AdminUser { get; set; } = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, Roles.Admin) }));

        public ClaimsPrincipal StartechLeaderUser { get; set; } = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(StartechClaimHelper.GetStartechClaimType(Startechs.Dotnet), StartechClaimHelper.Leader)
        }));

        public ClaimsPrincipal StartechMemberUser { get; set; } = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(StartechClaimHelper.GetStartechClaimType(Startechs.Dotnet), StartechClaimHelper.Member)
        }));

        public ClaimsPrincipal NoStartechUser { get; set; } = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>())); 


        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAuthorization(options => options.AddAppicationPolicies());
            services.AddStartechPoliciesHandler();
            AuthorizationService = services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        }

        [Test]
        public async Task Admin_policy_requires_Admin_role()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(AdminUser, Roles.Admin);
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task Admin_is_a_startech_leader()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(AdminUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true));
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task a_startech_leader_is_a_user_with_startechs_claims_and_value_Leader()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechLeaderUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true));
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task a_user_with_startechs_claims_and_value_Member_is_not_a_startech_leader()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechMemberUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true));
            authentificationResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task a_user_without_startechs_claims_is_not_a_startech_leader()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(NoStartechUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true));
            authentificationResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task a_startech_leader_is_a_startech_member()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechLeaderUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: false));
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task a_user_with_startechs_claims_and_value_Member_is_a_startech_member()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechMemberUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: false));
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task a_user_without_startechs_claims_is_not_a_startech_member()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(NoStartechUser, StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: false));
            authentificationResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task a_startech_leader_respects_AllStartechLeader_policy()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechLeaderUser, StartechPolicyHelper.AllStartechLeader);
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task if_a_user_is_not_a_startech_leader_he_dont_respect_AllStartechLeader_policy()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechMemberUser, StartechPolicyHelper.AllStartechLeader);
            authentificationResult.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task a_startech_member_respects_AllStartechmemebr_policy()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(StartechMemberUser, StartechPolicyHelper.AllStartechMember);
            authentificationResult.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task if_a_user_is_not_a_startech_Member_he_dont_respect_AllStartechMember_policy()
        {
            var authentificationResult = await AuthorizationService.AuthorizeAsync(NoStartechUser, StartechPolicyHelper.AllStartechMember);
            authentificationResult.Succeeded.Should().BeFalse();
        }
    }
}
