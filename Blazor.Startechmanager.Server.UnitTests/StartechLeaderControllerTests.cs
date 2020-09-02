using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
                    UserName = "Admin",
                    Roles = new List<ApplicationRole> { new ApplicationRole { Name = Roles.Admin } }
                });
            DbContext.Users.Add(
            new ApplicationUser
            {
                UserName = "Leader java",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java, IsLeader = true } }
            });
            DbContext.Users.Add(
            new ApplicationUser
            {
                UserName = "Leader dotnet",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true } }
            });
            DbContext.Users.Add(
              new ApplicationUser
              {
                  UserName = "member java",
                  Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java, IsLeader = false } }
              });
            DbContext.Users.Add(
            new ApplicationUser
            {
                UserName = "member dotnet",
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false } }
            });
            DbContext.Users.Add(
             new ApplicationUser
             {
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
        public void when_adding_admin_right_to_a_user_the_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void when_removing_admin_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ther_should_never_be_no_admin_in_the_application()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void when_adding_a_startech_leader_right_to_a_user_the_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void when_removing_a_startech_leader_right_to_a_user_the_removed_right_should_be_saved_in_database()
        {
            throw new NotImplementedException();
        }
    }
}
