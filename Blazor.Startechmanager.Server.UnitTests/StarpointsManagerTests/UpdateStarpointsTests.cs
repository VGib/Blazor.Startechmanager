using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class UpdateStarpointsTests : StarpointManagerControllerTests
    {
        private const int WorkingStarpointItemId = 999;

        private StarpointsItem CreateItemToUpdate(ValidationState state = ValidationState.InStudy, Startechs startech = Startechs.Dotnet, int userId = 5)
        {
            var starpoint = new StarpointsItem
            {
                Id = WorkingStarpointItemId,
                ApplicationUserId = userId,
                Startech = startech,
                Type = BlogArticle,
                NumberOfPoints = 15,
                ValidationState = state,
                Date = DateTime.Now
            };

            DbContext.Add(starpoint);
            DbContext.SaveChanges();

            return starpoint;
        }

        private StarpointsItem CreateInputStarpoint( StarpointsItem itemToUpdate, Action<StarpointsItem> todo = null )
        {
            var starpoint = new StarpointsItem
            {
                Id = itemToUpdate.Id,
                ApplicationUserId = itemToUpdate.ApplicationUserId,
                Startech = itemToUpdate.Startech,
                Type = itemToUpdate.Type
            };

            todo?.Invoke(starpoint);

            return starpoint;
        }

        [Test]
        public async Task UpdateStarpoints_only_valid_starpoints_should_be_updated()
        {
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate);
            var target = Create();
            target.ModelState.AddModelError("toto", "titi");
            var result = await target.UpdateStarpoints(inputStarpoint);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_url_or_description()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.TextJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpoint = DbContext.StarpointsItem.FirstOrDefault(x => x.Id == WorkingStarpointItemId);
            updatedStarpoint.TextJustification.Should().Be(updatedText);
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_url_or_description_2()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.UrlJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();
            updatedStarpoint.UrlJustification.Should().Be(updatedText);
        }

        private StarpointsItem GetUpdatedStarpoint()
        {
            return DbContext.StarpointsItem.FirstOrDefault(x => x.Id == WorkingStarpointItemId);
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_in_study_state()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Validated);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.UrlJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_in_study_state_2()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Refused);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.UrlJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_in_study_state_3()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.InStudy);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.UrlJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_update_his_starpoint_even_if_his_not_startech_member_anymore()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate(startech: Startechs.Agile);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.UrlJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_not_modify_starpoint_validation_status()
        {
            SetUser(MemberDotnet);
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.InStudy);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.ValidationState = ValidationState.Validated);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpointItem = GetUpdatedStarpoint();
            updatedStarpointItem.ValidationState.Should().Be(ValidationState.InStudy);
        }


        [Test]
        public async Task UpdateStarpoints_leader_can_not_modify_starpoint_validation_status()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Validated);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.ValidationState = ValidationState.Refused);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();
            updatedStarpoint.ValidationState.Should().Be(ValidationState.Validated);
            
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_not_modify_starpoint_number()
        {
            SetUser(MemberDotnet);
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.InStudy);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.NumberOfPoints = 1001);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpointItem = GetUpdatedStarpoint();
            updatedStarpointItem.ValidationState.Should().NotBe(1001);
        }

        [Test]
        public async Task UpdateStarpoints_leader_can_modify_starpoint_number()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Validated);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.NumberOfPoints = 200);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();
            updatedStarpoint.NumberOfPoints.Should().Be(200);
        }

        [Test]
        public async Task UpdateStarpoints_if_starpoints_have_been_validating_and_numberofstarpoints_has_been_modified_the_users_numberofstarpoints_should_differ_by_the_new_difference()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Validated);
            var oldNumberOfPoint = starpointToUpdate.NumberOfPoints;
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.NumberOfPoints = 200);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();

            var memberDotnet = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            memberDotnet.NumberOfPoints.Should().Be(200 - oldNumberOfPoint);
        }

        [Test]
        public async Task UpdateStarpoints_if_starpoints_have_been_validating_and_numberofstarpoints_has_been_modified_the_users_numberofstarpoints_should_differ_by_the_new_difference_2()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.InStudy);
            var oldNumberOfPoint = starpointToUpdate.NumberOfPoints;
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.NumberOfPoints = 200);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();

            var memberDotnet = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            memberDotnet.NumberOfPoints.Should().Be(0);
        }

        [Test]
        public async Task UpdateStarpoints_if_starpoints_have_been_validating_and_numberofstarpoints_has_been_modified_the_users_numberofstarpoints_should_differ_by_the_new_difference_3()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(state: ValidationState.Refused);
            var oldNumberOfPoint = starpointToUpdate.NumberOfPoints;
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.NumberOfPoints = 200);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();

            var memberDotnet = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            memberDotnet.NumberOfPoints.Should().Be(0);
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_date_can_not_be_modifyed()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate();
            var oldNumberOfPoint = starpointToUpdate.NumberOfPoints;
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.Date = new DateTime(2000,1,1));
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();
            updatedStarpoint.Date.Should().BeCloseTo(DateTime.Now, precision: 20000);
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_date_can_not_be_modifyed_2()
        {
            SetUser(MemberDotnet);
            var starpointToUpdate = CreateItemToUpdate();
            var oldNumberOfPoint = starpointToUpdate.NumberOfPoints;
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.Date = new DateTime(2000, 1, 1));
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            var updatedStarpoint = GetUpdatedStarpoint();
            updatedStarpoint.Date.Should().BeCloseTo(DateTime.Now, precision: 20000);
        }


        [Test]
        public async Task UpdateStarpoints_updates_the_starpoints_in_database()
        {
            SetUser(MemberDotnet);
            const string updatedText = "updated!!";
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.TextJustification = updatedText);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpoint = DbContext.StarpointsItem.FirstOrDefault(x => x.Id == WorkingStarpointItemId);
            updatedStarpoint.TextJustification.Should().Be(updatedText);
        }

        [Test]
        public async Task UpdateStarpoints_leader_can_modify_the_starpoint_type()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.Type = Presentation);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpoint = DbContext.StarpointsItem.FirstOrDefault(x => x.Id == WorkingStarpointItemId);
            updatedStarpoint.StarpointsTypeId.Should().Be(Presentation.Id);
        }


        [Test]
        public async Task UpdateStarpoints_member_can_not_modify_the_starpoint_type()
        {
            SetUser(MemberDotnet);
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.Type = Presentation);
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);

            var updatedStarpoint = DbContext.StarpointsItem.FirstOrDefault(x => x.Id == WorkingStarpointItemId);
            updatedStarpoint.StarpointsTypeId.Should().NotBe(Presentation.Id);
        }

        [Test]
        public async Task only_leader_can_update_starpoint_of_an_other_user()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate();
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.TextJustification = "update");
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task only_leader_can_update_starpoint_of_an_other_user_2()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var starpointToUpdate = CreateItemToUpdate(startech: Startechs.Java);
            var inputStarpoint = CreateInputStarpoint(starpointToUpdate, x => x.TextJustification = "update");
            var target = Create();
            var result = await target.UpdateStarpoints(inputStarpoint);
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
