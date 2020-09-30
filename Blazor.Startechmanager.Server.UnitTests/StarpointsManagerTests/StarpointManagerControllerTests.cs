using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Shared.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class StarpointManagerControllerTests : BaseTestsWithDbContext<StarPointsManagerController>
    {
        [SetUp]
        public void SetupValues()
        {
            DbContext.Add(new StarpointsType
            {
                Id = 1,
                TypeName = "Blog Article",
                NumberOfPoint = 15,
                IsActive = true
            });
            DbContext.Add(new StarpointsType
            {
                Id = 2,
                TypeName = "Course",
                NumberOfPoint = 150,
                IsActive = true
            });
            DbContext.Add(new StarpointsType
            {
                Id = 3,
                TypeName = "Presentation",
                NumberOfPoint = 80,
                IsActive = true
            });
            DbContext.Add(new StarpointsType
            {
                Id = 4,
                TypeName = "Obsolete",
                NumberOfPoint = 999,
                IsActive = false
            });
            DbContext.SaveChanges();
        }


        #region GetStarpoints
        [Test]
        public async Task GetStarpoints_should_return_only_starpoints_of_the_history_duration()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_should_return_the_current_users_starpoints_if_the_user_id_is_this_users_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_should_return_not_only_this_users_membership_starpoint()
        {
            throw new NotImplementedException("to do");
        }

        public async Task GetStarpoints_should_the_users_id_starpoint_if_user_id_is_not_this_user_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_should_the_users_id_starpoint_startechs_for_the_current_user_is_leader_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_if_current_user_is_not_startech_leader_and_is_trying_to_get_point_from_an_other_leader_a_notauthorized_error_message_should_be_answer()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_everybody_can_use_this_controller_action()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetStarpoints_should_return_the_starpoint_types ()
        {
            throw new NotImplementedException("to do");
        }

        #endregion

        #region GetInValidationStarpoints

        [Test]
        public async Task GetInValidationStarpoints_should_return_only_starpoints_to_validate_where_current_user_is_member_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task GetInValidationStarpoints_should_return_starpoints_to_validate_from_all_users()
        {
            throw new NotImplementedException("to do");
        }


        #endregion

        #region CreateStarpoints

        [Test]
        public async Task Only_valid_starpoints_should_be_created()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_only_user_of_the_startech_should_create_starpoints_for_current_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_if_the_currentuser_is_not_member_of_the_startech_he_can_not_create_starpoint_for_this_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_if_the_currentuser_is_not_leader_of_the_startech_he_can_not_create_starpoint_for_this_startech_and_a_other_user_which_is_member_of_the_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_even_startech_leader_can_not_create_starpoints_for_a_user_which_is_not_member_of_the_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_only_startech_leader_should_create_starpoint_for_another_user()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task CreateStarpoints_save_the_created_starpoints()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CreateStarpoints_only_startech_leader_can_create_starpoint_with_no_starpoints_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_a_startech_member_create_a_starpointsItem_with_a_starpoints_type_the_numberOfPoints_should_be_the_types_number_of_points()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task a_startech_leader_can_create_a_starpoint_item_with_a_type_and_a_different_starpoint_type()
        {
            throw new NotImplementedException("to do");
        }


        #endregion

        #region UpdateValidationStatus

        [Test]
        public async Task UpdateValidationStatus_should_be_used_only_by_this_startech_owner()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_should_not_be_used_by_a_startech_leader_from_an_other_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_should_save_the_new_validation_status()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_not_updated_from_Validation_no_starpoints_should_be_added_to_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_updated_to_validated_the_starpoints_should_be_added_to_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_removed_the_starpoint_should_be_removed_from_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_validating_the_same_status_should_be_an_error()
        {
            throw new NotImplementedException("to do");
        }

        #endregion

        #region UpdateStarpoints

        [Test]
        public async Task UpdateStarpoints_only_valid_starpoints_should_be_updated()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_url_or_description()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_not_validated()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task UpdateStarpoints_current_user_can_update_his_starpoint_even_if_his_not_startech_memeber_anymore()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_can_not_modify_starpoint_validation_status()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_not_modify_starpoint_number()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_startech_leader_can_modify_starpoints__number_only_if_starpoints_have_not_been_validated()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_users_starpoint_can_not_be_modifyed()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_date_can_not_be_modifyed()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_updates_the_starpoints_in_database()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_only_startech_leader_can_modify_the_starpoint_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_type_can_be_modifyed_only_on_InValidation_state()
        {
            throw new NotImplementedException("to do");
        }

        #endregion

        #region CancelStarpoints

        [Test]
        public async Task CancelStarpoints_only_inValidation_starpoints_can_be_cancelled()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_his_starpoints()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_cancel_his_starpoint_even_if_is_not_member_of_the_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_from_other_user_for_startech_where_is_member_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_removes_starpoints_from_database()
        {
            throw new NotImplementedException("to do");
        }


        #endregion

    }
}
