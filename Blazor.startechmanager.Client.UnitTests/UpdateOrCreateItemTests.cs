using Blazor.Startechmanager.Client.Pages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class UpdateOrCreateItemTests : BaseTestsForComponent<UpdateOrCreateItem>
    {
        [Test]
        public async Task for_new_item_a_new_StarpointItem_should_be_created()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task for_new_item_created_by_this_user_a_new_StarpointItem_should_be_created_with_current_users_true_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_created_by_a_leader_a_new_StarpointItem_should_be_created_with_the_users_item_creators_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_updated_item_created_by_this_user_the_item_should_be_load_from_server()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_updated_item_created_by_this_user_the_items_user_should_be_load()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_created_by_this_user_this_current_user_should_be_load()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_created_by_a_leader_the_user_should_be_load()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_created_by_this_user_available_startech_should_be_the_startechs_from_which_user_is_member_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_created_for_another_user_available_startech_should_be_retrieved_from_server()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_updated_item_created_for_this_user_available_startech_should_be_the_startechs_from_which_user_is_member_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_updated_item_created_for_another_user_available_startech_should_be_retrieved_from_server()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task startech_current_user_is_leader_of_should_be_load_by_startech_which_current_user_is_leader_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_all_has_been_load_is_load_should_be_true_create_item()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_all_has_been_load_is_load_should_be_true_update_item()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_new_item_isNew_should_be_true()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task for_updated_item_isNew_should_be_false()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task when_creating_an_item_with_this_user_createItem_service_should_be_called_with_this_user_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_creating_an_item_with_an_other_user_createItem_service_should_be_called_with_the_user_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_updating_an_item_with_this_user_updateItem_service_should_be_called_with_this_user_id()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_updating_an_item_with_an_other_user_updateItem_service_should_be_called_with_the_user_id()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task when_creating_an_item_with_this_user_createItem_service_should_navigate_to_this_users_points()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_creating_an_item_with_an_other_user_createItem_service_should_navigate_to_the_users_points()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_updating_an_item_with_this_user_updateItem_service_should_navigate_to_this_user_points()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_updating_an_item_with_an_other_user_updateItem_service_should_navigate_to_the_users_points()
        {
            throw new NotImplementedException("to do");
        }
    }
}
