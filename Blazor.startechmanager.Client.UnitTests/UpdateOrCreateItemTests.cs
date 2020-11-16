﻿using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Shared.Constants;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class UpdateOrCreateItemTests : BaseTestsForComponent<UpdateOrCreateItem>
    {
        public ComponentParameter ThisuserId { get; set; } = ComponentParameterFactory.Parameter("UserId", ThisUser.Id);

        public int ThisUserDatabaseId { get; set; } = 19;

        [Test]
        public async Task for_new_item_a_new_StarpointItem_should_be_created()
        {
            InitializeHttpCallNewItem(ThisuserId);
            var target = CreateComponent(ThisuserId);
            await Task.Delay(30);
            target.Instance.Item.Should().NotBeNull();
        }

        private void InitializeHttpCallNewItem(ComponentParameter userIdParameter)
        {
            int userId = (int)userIdParameter.Value;
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{userIdParameter}")
                .Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = userId == ThisUser.Id ? ThisUserDatabaseId : userId, UserName = "User" }));
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetStarpointsType/-1")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsType { Id = 1 } }));
            if(userId != ThisUser.Id)
            {
                MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUserStartechs/{userId}")
                    .Respond("application/json", JsonSerializer.Serialize( new[] { Startechs.Dotnet }));
            }
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
