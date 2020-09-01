using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class StartechOwnerControllerTests : BaseTests
    {
        [Test]
        public void Only_admin_can_use_the_controller_for_modifying_admins_rights()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void NonAdmin_admin_cant_use_the_controller_for_modifying_admins_rights()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void admin_can_use_the_controller_for_modifying_some_startech_rights()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Startech_leaders_can_use_the_controller_for_modifying_startechs_rights()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Users_not_admin_and_not_startech_leaders_can_not_use_the_controller_for_modifying_startechs_rights()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void when_getting_the_admin_users_all_users_who_can_admin_the_application_should_be_return()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void when_getting_the_startechs_users_all_users_who_can_have_startechs_point_should_be_return()
        {
            throw new NotImplementedException();
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
