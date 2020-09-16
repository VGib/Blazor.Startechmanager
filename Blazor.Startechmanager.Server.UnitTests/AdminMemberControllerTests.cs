using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Common.UnitTests;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using route = Microsoft.AspNetCore.Routing;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class AuthorizeAdminMembeTests : BaseTests<AuthorizeAdminMember>
    {

        public Mock<IAuthorizationService> AuthorizationService { get; set; }

        [Test]
        public async Task only_the_startech_leader_of_the_same_startech_than_route_urls_startech_can_admin_the_members()
        {
            MockAuthorizationService(AuthorizationResult.Success());
            AuthorizationFilterContext context = CreateHttpContext();

            var target = Create();
            await target.OnAuthorizationAsync(context);
            context.Result.Should().BeNull();


        }

        private void MockAuthorizationService(AuthorizationResult result)
        {
            var policyToCall = StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn<string>(policyToCall)))
                .Returns(Task.Factory.StartNew(() => result));
        }

        private static AuthorizationFilterContext CreateHttpContext()
        {
            return new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal()
            }, new route.RouteData(new route.RouteValueDictionary
            {
                ["startechType"] = "Dotnet"
            }), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor())
                ,Array.Empty<IFilterMetadata>());
        }

        [Test]
        public async Task user_which_are_not_startech_leaders_cant_admin_the_members()
        {
            MockAuthorizationService(AuthorizationResult.Failed());
            AuthorizationFilterContext context = CreateHttpContext();

            var target = Create();
            await target.OnAuthorizationAsync(context);
            context.Result.Should().BeOfType<UnauthorizedResult>();
        }
    }

    public class AdminMemberControllerTests
    {
    }
}
