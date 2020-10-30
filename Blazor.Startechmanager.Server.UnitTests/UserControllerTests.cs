using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Constants;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class UserControllerTests : BaseTestsWithDbContext<UserController>
    {
        public Mock<UserManager<ApplicationUser>> UserManager { get; set; }

        public Mock<IAuthorizationService> AuthorizationService { get; set; }

        protected void SetUser(ApplicationUser user)
        {
            UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.Factory.StartNew(() => user));
        }

        protected void AuthorizeLeader()
        {
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(StartechPolicyHelper.AllStartechLeader)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(policy)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));

            foreach (var startech in Enum.GetValues(typeof(Startechs)).Cast<Startechs>().Where(x => x != Startechs.Dotnet))
            {
                var ThisLoopPolicy = StartechPolicyHelper.GetPolicyName(startech, MustBeLeader: true);
                AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(ThisLoopPolicy)))
                    .Returns(Task.Factory.StartNew(() => AuthorizationResult.Failed()));
            }
        }

        protected override void SetMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            UserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            ServiceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = new DefaultHttpContext() });
        }

        protected readonly ApplicationUser MemberDotnet = new ApplicationUser
        {
            Id = 5,
            UserName = "Member dotnet",
            Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false, ApplicationUserId = 3 },
                                    new MappingStartechUser { Startech = Startechs.Java, IsLeader = false, ApplicationUserId = 3 }
            }
        };

        protected readonly ApplicationUser User = new ApplicationUser
        {
            Id = 7,
            UserName = "User"
        };

        [SetUp]
        public void FillDatas()
        {
            DbContext.Users.Add(User);
            DbContext.Users.Add(MemberDotnet);
            DbContext.SaveChanges();
        }

        [Test]
        public async Task GetUser_should_return_this_user_userObject_when_userId_is_thisUser()
        {
            SetUser(User);
            var target = Create();
            var user = await target.GetUser(ThisUser.Id);
            user.Id.Should().Be(User.Id);
        }

        [Test]
        public async Task GetUser_should_return_this_user_userObject_when_userId_is_current_user_id()
        {
            SetUser(User);
            var target = Create();
            var user = await target.GetUser(User.Id);
            user.Id.Should().Be(User.Id);
        }

        [Test]
        public async Task GetUser_the_userId_users_object()
        {
            AuthorizeLeader();
            var target = Create();
            var user = await target.GetUser(MemberDotnet.Id);
            user.Id.Should().Be(MemberDotnet.Id);
        }

        [Test]
        public async Task GetUser_only_users_startech_member_where_caller_is_leader_can_return_the_userObject_for_another_user()
        {
            AuthorizeLeader();
            var target = Create();
            Action action = () => target.GetUser(User.Id).GetAwaiter().GetResult();
            action.Should().Throw<UserControllerException>();
        }

        [Test]
        public async Task GetUser_this_users_userobject_can_be_return_by_every_user()
        {
            SetUser(User);
            var target = Create();
            var user = await target.GetUser(ThisUser.Id);
            user.Id.Should().Be(User.Id);
        }

        [Test]
        public async Task GetUserStartechs_return_only_startech_for_which_caller_is_startech_leader()
        {
            AuthorizeLeader();
            var target = Create();
            var startechs = await target.GetUserStartechs(MemberDotnet.Id);
            startechs.Should().BeEquivalentTo(Startechs.Dotnet);
        }
    }
}
