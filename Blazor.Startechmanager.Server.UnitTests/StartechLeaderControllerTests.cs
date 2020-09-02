using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class StartechLeaderControllerTests : BaseTests<StartechLeaderController>
    {
        public Mock<UserManager<ApplicationUser>> UserManager { get; set; }

        protected override void SetMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            UserManager =  new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task when_getting_the_admin_users_all_users_who_can_admin_the_application_should_be_return()
        {
            PopulateDatas();
            var target = Create();
            var output = await target.GetLeaders("Admin");
            output.Select(x => x.UserName).Should().BeEquivalentTo("Admin");
        }

        private void PopulateDatas()
        {
            DbContext.Users.Add(
                new ApplicationUser
                {
                    Id = 1,
                    UserName = "Admin",
                    Roles = new List<ApplicationRole> { new ApplicationRole { Name = Roles.Admin } }
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
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true } }
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
            PopulateDatas();
            var target = Create();
            var output = await target.GetLeaders(Startechs.Dotnet.ToString());
            output.Select(x => x.UserName).Should().BeEquivalentTo("Leader dotnet");
        }

        [Test]
        public async Task when_getting_the_admin_users_all_users_who_can_not_admin_the_application_should_not_be_return()
        {
            PopulateDatas();
            var target = Create();
            var output = await target.GetLeaders("Admin");
            output.Select(x => x.UserName).Should().BeEquivalentTo("Admin");
        }

        [Test]
        public async Task when_getting_the_startechs_users_all_users_who_are_not_startech_leader_should_not_be_return()
        {
            PopulateDatas();
            var target = Create();
            var output = await target.GetLeaders(Startechs.Dotnet.ToString());
            output.Select(x => x.UserName).Should().BeEquivalentTo("Leader dotnet");
        }

        [Test]
        public async Task when_adding_admin_right_to_a_user_the_right_should_be_saved_in_database()
        {
            PopulateDatas();
            UserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)))
            .Returns(Task.Factory.StartNew(() => new IdentityResult()));
            var target = Create();
            await target.AddLeader("Admin", 6);

            UserManager.Verify(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)));

        }

        [Test]
        public void when_removing_admin_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void their_should_never_be_no_admin_in_the_application()
        {
            throw new NotImplementedException();
        }


        [Test]
        public async Task when_adding_a_startech_leader_right_to_a_user_the_right_should_be_saved_in_database()
        {
            PopulateDatas();
            var target = Create();
            await target.AddLeader(Startechs.Dotnet.ToString(), 6);

            DiscardDbContextChanges();
            DbContext.MappingStartechs.Any(x => x.UserId == 6 && x.Startech == Startechs.Dotnet && x.IsLeader).Should().BeTrue();
        }

        [Test]
        public void when_removing_a_startech_leader_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task when_adding_admin_right_a_user_should_not_be_added_twice()
        {
            PopulateDatas();
            UserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)))
            .Returns(Task.Factory.StartNew(() => new IdentityResult()));
            var target = Create();
            await target.AddLeader("Admin", 6);

            UserManager.Verify(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x.UserName == "nothing"), It.Is<string>(x => x == Roles.Admin)));

        }

        [Test]
        public async Task  when_adding_a_star_tech_leader_right_a_user_should_not_be_added_twice()
        {
            PopulateDatas();
            var target = Create();
            await target.AddLeader(Startechs.Dotnet.ToString(), 3);

            DiscardDbContextChanges();
            DbContext.MappingStartechs.Count(x => x.UserId == 3 && x.Startech == Startechs.Dotnet).Should().Be(1);

        }

        [Test]
        public async Task a_startech_member_can_be_upgrader_to_startech_leader()
        {
            PopulateDatas();
            var target = Create();
            await target.AddLeader(Startechs.Dotnet.ToString(), 5);

            DiscardDbContextChanges();
            DbContext.MappingStartechs.Count(x => x.UserId == 5 && x.Startech == Startechs.Dotnet).Should().Be(1);
            DbContext.MappingStartechs.Any(x => x.UserId == 5 && x.Startech == Startechs.Dotnet && x.IsLeader).Should().BeTrue();
        }
    }
}
