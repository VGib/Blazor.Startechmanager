using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class CancelStarpointsTests : StarpointManagerControllerTests
    {
         private const int WorkingStarpointItemId = 999;

        private StarpointsItem CreateItemToCancel(ValidationState state = ValidationState.InStudy, Startechs startech = Startechs.Dotnet,int userId = 5)
        {
            var starpoint = new StarpointsItem
            {
                Id = WorkingStarpointItemId,
                ApplicationUserId = userId,
                Startech = startech,
                NumberOfPoints = 15,
                ValidationState = state,
                Date = DateTime.Now
            };

            DbContext.Add(starpoint);
            DbContext.SaveChanges();

            return starpoint;
        }

        [Test]
        public async Task CancelStarpoints_only_inStudy_starpoints_can_be_cancelled()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(state: ValidationState.InStudy);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<OkResult>();

        }

        [Test]
        public async Task CancelStarpoints_only_inStudy_starpoints_can_be_cancelled_2()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(state: ValidationState.Refused);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<BadRequestObjectResult>();

        }

        [Test]
        public async Task CancelStarpoints_only_inStudy_starpoints_can_be_cancelled_3()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(state: ValidationState.Validated);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<BadRequestObjectResult>();

        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_his_starpoints()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(userId: MemberDotnet.Id);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_his_starpoints_2()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(userId: User.Id);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_cancel_his_starpoint_even_if_is_not_member_of_the_startech()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel(startech: Startechs.Agile);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_from_other_user_for_startech_where_is_member_of()
        {
            SetUser(LeaderDotnet);

            var starpoint = CreateItemToCancel(startech: Startechs.Dotnet);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_from_other_user_for_startech_where_is_member_of_2()
        {
            SetUser(LeaderDotnet);

            var starpoint = CreateItemToCancel(startech: Startechs.Java);
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CancelStarpoints_removes_starpoints_from_database()
        {
            SetUser(MemberDotnet);

            var starpoint = CreateItemToCancel();
            var target = Create();

            var result = await target.CancelStarpoints(WorkingStarpointItemId);

            DbContext.StarpointsItem.Any(x => x.Id == WorkingStarpointItemId).Should().BeFalse();
        }


    }
}
