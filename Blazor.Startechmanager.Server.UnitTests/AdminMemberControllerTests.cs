using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Common.UnitTests;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
                , Array.Empty<IFilterMetadata>());
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

    public class AdminMemberControllerTests : BaseTestsWithDbContext<AdminMemberController>
    {
        [SetUp]
        public void SetUpDatas()
        {
            DbContext.Users.Add(
           new ApplicationUser
           {
               Id = 3,
               UserName = "Leader dotnet",
               Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true, ApplicationUserId = 3 } }
           });
            DbContext.Users.Add(
              new ApplicationUser
              {
                  Id = 4,
                  UserName = "member dotnet 1",
                  Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false } }
              });
            DbContext.Users.Add(
            new ApplicationUser
            {
                Id = 5,
                UserName = "member dotnet 2",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false } }
            });
            DbContext.Users.Add(
               new ApplicationUser
               {
                   Id = 6,
                   UserName = "member java",
                   Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java, IsLeader = false } }
               });
            DbContext.Users.Add(
           new ApplicationUser
           {
               Id = 7,
               UserName = "user"
           });
            DbContext.SaveChanges();
        }

        [Test]
        public async Task GetMembers_should_retrieve_all_startech_member()
        {
            var target = Create();
            var members = await target.GetMembers(Startechs.Dotnet);
            members.Select(x => x.UserName).Should().BeEquivalentTo("member dotnet 1", "member dotnet 2");
        }

        [Test]
        public async Task SetMember_should_add_startech_member_to_a_user()
        {
            var target = Create();
            await target.SetMember(Startechs.Dotnet, 7);
            DbContext.MappingStartechs.Any(x => x.ApplicationUserId == 7 && x.Startech == Startechs.Dotnet && !x.IsLeader).Should().BeTrue();
        }

        [Test]
        public async Task SetMember_should_not_add_startech_member_twice_to_a_user()
        {
            var target = Create();
            await target.SetMember(Startechs.Dotnet, 4);
            DbContext.MappingStartechs.Count(x => x.ApplicationUserId == 4 && x.Startech == Startechs.Dotnet).Should().Be(1);
        }

        [Test]
        public async Task SetMember_should_not_remove_Leader_right()
        {
            var target = Create();
            await target.SetMember(Startechs.Dotnet, 3);
            DbContext.MappingStartechs.Count(x => x.ApplicationUserId == 3 && x.Startech == Startechs.Dotnet && x.IsLeader).Should().Be(1);
        }

        [Test]
        public async Task RemoveMember_should_remove_the_membership_of_a_member()
        {
            var target = Create();
            await target.RemoveMember(Startechs.Dotnet, 4);
            DbContext.MappingStartechs.Any(x => x.ApplicationUserId == 4 && x.Startech == Startechs.Dotnet).Should().BeFalse();

        }

        [Test]
        public async Task RemoveMember_should_return_an_error_result_if_user_is_not_a_member()
        {
            var target = Create();
            var result = await target.RemoveMember(Startechs.Dotnet, 7);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task RemobeMember_can_not_remove_a_leadership()
        {
            var target = Create();
            var result = await target.RemoveMember(Startechs.Dotnet, 3);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
