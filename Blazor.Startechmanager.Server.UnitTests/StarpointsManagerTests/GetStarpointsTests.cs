using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Shared.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class GetStarpointsTests :StarpointManagerControllerTests
    {
        [Test]
        public async Task GetStarpoints_if_no_history_is_in_input_default_history_should_be_2_years()
        {
            SetUser(LeaderDotnet);
            var target = Create();
            var result = await target.GetStarpoints(ThisUser.Id, null);
            result.Select(x => x.Id).Should().BeEquivalentTo(1, 2, 3);
        }

        [Test]
        public async Task GetStarpoints_should_return_only_starpoints_of_the_history_duration()
        {
            SetUser(LeaderDotnet);
            var target = Create();
            var result = await target.GetStarpoints(ThisUser.Id, TimeSpan.FromDays(8 * 365));
            result.Select(x => x.Id).Should().BeEquivalentTo(1, 2, 3, 4);
        }

        [Test]
        public async Task GetStarpoints_should_return_the_current_users_starpoints_if_the_user_id_is_this_users_id()
        {
            SetUser(MemberDotnet);
            var target = Create();
            var result = await target.GetStarpoints(ThisUser.Id, null);
            result.Select(x => x.Id).Should().BeEquivalentTo(6, 7, 8, 9, 10, 11);
        }

        [Test]
        public async Task GetStarpoints_should_return_not_only_this_users_membership_starpoint()
        {
            SetUser(MemberDotnet);
            var target = Create();
            var result = await target.GetStarpoints(ThisUser.Id, null);
            result.Select(x => x.Id).Should().BeEquivalentTo(6, 7, 8,9,10, 11);
        }

        [Test]
        public async Task GetStarpoints_should_the_users_id_starpoint_if_user_id_is_not_this_user_id()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();
            var result = await target.GetStarpoints(MemberDotnet.Id, null);
            result.Select(x => x.Id).Should().BeEquivalentTo(6, 7, 8, 10);
        }

        [Test]
        public async Task GetStarpoints_should_the_users_id_starpoint_startechs_for_the_current_user_is_leader_of()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();
            var result = await target.GetStarpoints(LeaderDotnet.Id, null);
            result.Select(x => x.Id).Should().BeEquivalentTo(1,2);
        }

        [Test]
        public async Task GetStarpoints_if_current_user_is_not_startech_leader_and_is_trying_to_get_point_from_an_other_user_a_notauthorized_error_message_should_be_answer()
        {
            SetUser(MemberDotnet);
            UnauthorizeMember();
            var target = Create();
            Action action = () => target.GetStarpoints(LeaderDotnet.Id, null).GetAwaiter().GetResult();
            action.Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public async Task GetStarpoints_everybody_can_use_this_controller_action()
        {
            var thisMethod = typeof(StarPointsManagerController).GetMethod(nameof(StarPointsManagerController.GetStarpoints));
            var attributes = thisMethod.GetCustomAttributes(false);
            attributes.Where(x => x.GetType().IsAssignableFrom(typeof(IAuthorizationFilter)) || x.GetType().IsAssignableFrom(typeof(IAsyncAuthorizationFilter))).Should().BeEmpty();
        }

        [Test]
        public async Task GetStarpoints_should_return_the_starpoint_types()
        {
            SetUser(LeaderDotnet);
            var target = Create();
            var result = await target.GetStarpoints(ThisUser.Id, null);
            result.First(x => x.Id == 1).Type.Should().NotBeNull();
        }
    }
}
