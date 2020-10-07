using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class UpdateStarpointsTests : StarpointManagerControllerTests
    {
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
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_in_study_state()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task UpdateStarpoints_current_user_can_update_his_starpoint_even_if_his_not_startech_member_anymore()
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
        public async Task UpdateStarpoints_if_starpoints_have_been_validating_and_numberofstarpoints_has_been_modified_the_users_numberofstarpoints_should_differ_by_the_new_difference()
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
    }
}
