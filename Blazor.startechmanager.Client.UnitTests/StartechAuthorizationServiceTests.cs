using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Common.UnitTests;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class StartechAuthorizationServiceTests : BaseTests<StartechAuthorizationService>
    {
        public class CustomAuthentificationProvider: AuthenticationStateProvider
        {
            public override Task<AuthenticationState> GetAuthenticationStateAsync()
            {
                return Task.Factory.StartNew(() => new AuthenticationState(new System.Security.Claims.ClaimsPrincipal()));
            }
        }

        public AuthenticationStateProvider AuthenticationProvider { get; set; } = new CustomAuthentificationProvider();

        public Mock<IAuthorizationService> AuthorizationService { get; set; }

        [Test]
        public async Task StartechAuthorizationService_should_ask_authorization_with_correct_policy()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, true);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            AuthorizationService.Verify(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),It.IsAny<object>(), It.IsIn(policy)));
        }

        [Test]
        public async Task when_user_is_not_member_StartechAuthorizationService_should_return_false()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, false);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Failed()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            result.Should().BeFalse();
        }

        [Test]
        public async Task when_user_is_member_and_must_be_leader_is_false_StartechAuthorizationService_should_return_true()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, false);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
              .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            result.Should().BeTrue();
        }

        [Test]
        public async Task when_user_is_member_but_not_leader_and_must_be_leader_is_true_StartechAuthorizationService_should_return_false()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, true);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
              .Returns(Task.Factory.StartNew(() => AuthorizationResult.Failed()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            result.Should().BeFalse();
        }

        [Test]
        public async Task when_user_is_leader_and_must_be_leader_is_false_StartechAuthorizationService_should_return_true()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, false);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
              .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            result.Should().BeTrue();
        }

        [Test]
        public async Task when_user_is_leader_and_must_be_leader_is_true_StartechAuthorizationService_should_return_true()
        {
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Agile, false);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
              .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var target = Create();
            var result = await target.IsMemberOrLeaderOf(Startechs.Agile);
            result.Should().BeTrue();
        }
    }
}
