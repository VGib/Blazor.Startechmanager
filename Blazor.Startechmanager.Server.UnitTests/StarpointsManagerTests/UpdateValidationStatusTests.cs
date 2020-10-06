using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class UpdateValidationStatusTests : StarpointManagerControllerTests
    {
        private const int WorkingStarpointItemId = 999;
        private const int NumberOfPointOfWorkingStapointItem = 101;

        private StarpointsItem CreateItemToUpdateStatus( ValidationState state = ValidationState.InStudy,  Startechs  startech = Startechs.Dotnet)
        {
            var starpoint = new StarpointsItem
            {
                Id = WorkingStarpointItemId,
                ApplicationUserId = MemberDotnet.Id,
                Startech = startech,
                NumberOfPoints = NumberOfPointOfWorkingStapointItem,
                ValidationState = state,
                Date = DateTime.Now
            };

            DbContext.Add(starpoint);
            DbContext.SaveChanges();

            return starpoint;
        }

        [SetUp]
        public void  AuthorizeAndSetLeaderForAllTests()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
        }

        [Test]
        public async Task UpdateValidationStatus_should_be_used_only_by_this_startech_leader()
        {
            CreateItemToUpdateStatus(startech: Startechs.Dotnet);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.Validated);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task UpdateValidationStatus_should_not_be_used_by_a_startech_leader_from_an_other_startech()
        {
            CreateItemToUpdateStatus(startech: Startechs.Java);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.Validated);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateValidationStatus_should_save_the_new_validation_status()
        {
            CreateItemToUpdateStatus(state: ValidationState.InStudy);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.Validated);

            var updatedStartechItem = DbContext.StarpointsItem.First(x => x.Id == WorkingStarpointItemId);
            updatedStartechItem.ValidationState.Should().IsSameOrEqualTo(ValidationState.Validated);
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_not_updated_from_Validation_no_starpoints_should_be_added_to_the_user()
        {
            CreateItemToUpdateStatus(state: ValidationState.InStudy);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.Refused);

            var user = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            user.NumberOfPoints.Should().Be(0);
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_updated_to_validated_the_starpoints_should_be_added_to_the_user()
        {
            CreateItemToUpdateStatus(state: ValidationState.InStudy);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.Validated);

            var user = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            user.NumberOfPoints.Should().Be(NumberOfPointOfWorkingStapointItem);
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_removed_the_starpoint_should_be_removed_from_the_user()
        {
            CreateItemToUpdateStatus(state: ValidationState.Validated);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.InStudy);

            var user = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            user.NumberOfPoints.Should().Be( - NumberOfPointOfWorkingStapointItem);
        }

        [Test]
        public async Task UpdateValidationStatus_validating_the_same_status_should_be_an_error()
        {
            CreateItemToUpdateStatus(state: ValidationState.InStudy);

            var target = Create();
            var result = await target.UpdateValidationStatus(WorkingStarpointItemId, ValidationState.InStudy);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
