using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class CreateStarpointsTests
    {
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
        public async Task a_leader_can_not_create_starpoints_from_a_non_active_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task a_member_can_not_create_starpoints_from_a_non_active_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task a_startech_leader_can_create_a_starpoint_item_with_a_type_and_a_different_starpoint_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_a_leader_create_a_starpoint_the_starpoint_state_is_validated()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_a_member_create_a_starpoint_the_starpoint_state_is_in_study()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task if_the_user_is_current_user_the_starpoint_should_be_created_for_him()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task if_another_user_is_in_parameter_the_starpoint_should_be_created_for_this_user()
        {
            throw new NotImplementedException("to do");
        }
    }
}
