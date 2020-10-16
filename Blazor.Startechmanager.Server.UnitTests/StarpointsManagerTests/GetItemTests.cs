using Blazor.Startechmanager.Server.Controllers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class GetItemTests : StarpointManagerControllerTests
    {
        [Test]
        public async  Task the_input_item_should_be_return()
        {
            SetUser(MemberDotnet);
            var target = Create();
            var item = await target.GetItem(6);
            item.Id.Should().Be(6);
        }

        [Test]
        public async Task current_user_can_return_one_of_his_item()
        {
            SetUser(MemberDotnet);
            var target = Create();
            var item = await target.GetItem(6);
            item.Id.Should().Be(6);
        }

        [Test]
        public async Task current_user_can_return_one_of_his_item_even_if_is_not_startech_member()
        {
            SetUser(User);
            var target = Create();
            var item = await target.GetItem(12);
            item.Id.Should().Be(12);
        }

        [Test]
        public async Task current_user_which_is_not_startech_leader_can_not_return_another_users_item()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();
            Action action = ( )=> target.GetItem(9).GetAwaiter().GetResult();
            action.Should().Throw<StarpointManagerException>();
        }


        [Test]
        public async Task startech_leaders_can_return_item_for_a_startech_member()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();
            var item = await target.GetItem(6);
            item.Id.Should().Be(6);
        }
    }
}
