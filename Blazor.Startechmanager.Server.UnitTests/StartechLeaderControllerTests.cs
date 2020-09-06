using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class StartechLeaderControllerTests : BaseTestsWithDbContext<StartechLeaderController>
    {
        public Mock<UserManager<ApplicationUser>> UserManager { get; set; }

        protected override void SetMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            UserManager =  new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            ServiceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = new DefaultHttpContext() });
        }

        [Test]
        public async Task when_getting_the_admin_users_all_users_who_can_admin_the_application_should_be_return()
        {
            UserManager.Setup(x => x.GetUsersInRoleAsync(Roles.Admin))
                .Returns(Task.Factory.StartNew<IList<ApplicationUser>>(() => DbContext.Users.Where(x => x.UserName == "Admin").ToList()));
            var target = Create();
            var output = await target.GetLeaders("Admin");
            output.Select(x => x.UserName).Should().BeEquivalentTo("Admin");
        }

        [SetUp]
        public void PopulateDatas()
        {
            DbContext.Users.Add(
                new ApplicationUser
                {
                    Id = 1,
                    UserName = "Admin"
                });
            DbContext.Users.Add(
            new ApplicationUser
            {
                Id = 2,
                UserName = "Leader java",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java, IsLeader = true } }
            });
            DbContext.Users.Add(
            new ApplicationUser
            {
                Id = 3,
                UserName = "Leader dotnet",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true, ApplicationUserId = 3} }
            });
            DbContext.Users.Add(
              new ApplicationUser
              {
                  Id = 4,
                  UserName = "member java",
                  Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java, IsLeader = false } }
              });
            DbContext.Users.Add(
            new ApplicationUser
            {
                Id = 5,
                UserName = "member dotnet",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false } }
            });
            DbContext.Users.Add(
             new ApplicationUser
             {
                 Id = 6,
                 UserName = "nothing"
             });
            DbContext.SaveChanges();
        }

        [Test]
        public async Task when_getting_the_startechs_users_all_users_who_are_startech_leader_should_be_return()
        {
            var target = Create();
            var output = await target.GetLeaders(Startechs.Dotnet.ToString());
            output.Select(x => x.UserName).Should().BeEquivalentTo("Leader dotnet");
        }

        [Test]
        public async Task when_getting_the_admin_users_all_users_who_can_not_admin_the_application_should_not_be_return()
        {
            await when_getting_the_admin_users_all_users_who_can_admin_the_application_should_be_return();
        }

        [Test]
        public async Task when_getting_the_startechs_users_all_users_who_are_not_startech_leader_should_not_be_return()
        {
            var target = Create();
            var output = await target.GetLeaders(nameof(Startechs.Dotnet));
            output.Select(x => x.UserName).Should().BeEquivalentTo("Leader dotnet");
        }

        [Test]
        public async Task when_adding_admin_right_to_a_user_the_right_should_be_saved_in_database()
        { 
            UserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)))
            .Returns(Task.Factory.StartNew(() => new IdentityResult()));
            var target = Create();
            await target.AddLeader("Admin", 6);

            UserManager.Verify(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)));
        }

        [Test]
        public async Task when_removing_admin_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            DbContext.Add(new ApplicationUser
            {
                Id = 10,
                UserName = "Admin to remove",
            });
            DbContext.SaveChanges();
            UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(() => Task.Factory.StartNew(() => DbContext.Users.First(x => x.Id == 1)));
            var target = Create();
            await target.RemoveLeader(Roles.Admin, 10);
            UserManager.Verify(x => x.RemoveFromRoleAsync(It.Is<ApplicationUser>(x => x.Id == 10), It.Is<string>(x => x == Roles.Admin)));
        }

        [Test]
        public async Task  their_should_never_be_no_admin_in_the_application()
        {
           await i_cant_remove_my_own_admin_privilege();
        }

        [Test]
        public async Task when_adding_a_startech_leader_right_to_a_user_the_right_should_be_saved_in_database()
        {
            var target = Create();
            await target.AddLeader(nameof(Startechs.Dotnet), 6);

            DbContext.MappingStartechs.Any(x => x.ApplicationUserId == 6 && x.Startech == Startechs.Dotnet && x.IsLeader).Should().BeTrue();
        }

        [Test]
        public async Task when_removing_a_startech_leader_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(() => Task.Factory.StartNew(() => DbContext.Users.First(x => x.Id == 1)));
            var target = Create();
            await target.RemoveLeader(nameof(Startechs.Dotnet), 3);

            DbContext.MappingStartechs.First(x => x.Startech == Startechs.Dotnet && x.ApplicationUserId == 3).IsLeader.Should().BeFalse();
        }

        [Test]
        public async Task i_cant_remove_my_own_admin_privilege()
        {
            UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(() => Task.Factory.StartNew(() => DbContext.Users.First(x => x.Id == 1)));
            var target = Create();
            var result = await target.RemoveLeader(Roles.Admin, 1);
            result.Should().BeOfType(typeof(BadRequestObjectResult));
            UserManager.Invocations.Any(x => x.Method.Name == nameof(UserManager<ApplicationUser>.RemoveFromRoleAsync)).Should().BeFalse();
        }

        [Test]
        public async Task when_adding_admin_right_a_user_should_not_be_added_twice()
        {
            UserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)))
            .Returns(Task.Factory.StartNew(() => new IdentityResult()));
            var target = Create();
            await target.AddLeader("Admin", 6);

            UserManager.Verify(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)));
        }

        [Test]
        public async Task  when_adding_a_star_tech_leader_right_a_user_should_not_be_added_twice()
        {
            var target = Create();
            await target.AddLeader(nameof(Startechs.Dotnet), 3);

            DbContext.MappingStartechs.Count(x => x.ApplicationUserId == 3 && x.Startech == Startechs.Dotnet).Should().Be(1);
        }

        [Test]
        public async Task a_startech_member_can_be_upgrader_to_startech_leader()
        {
            var target = Create();
            await target.AddLeader(nameof(Startechs.Dotnet), 5);

            DbContext.MappingStartechs.Count(x => x.ApplicationUserId == 5 && x.Startech == Startechs.Dotnet).Should().Be(1);
            DbContext.MappingStartechs.Any(x => x.ApplicationUserId == 5 && x.Startech == Startechs.Dotnet && x.IsLeader).Should().BeTrue();
        }
    }
}
