using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Constants;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class UpdateOrCreateItemTests : BaseTestsForComponent<UpdateOrCreateItem>
    {
        public ComponentParameter ThisUserId { get; set; } = ComponentParameterFactory.Parameter("UserId", ThisUser.Id);

        public ComponentParameter OtherUserId { get; set; } = ComponentParameterFactory.Parameter("UserId", 12);

        public ComponentParameter TestItemId { get; set; } = ComponentParameterFactory.Parameter("ItemId", 107);

        public Mock<IMessageDisplayer> MessageDisplayer { get; set; }

        public Mock<IStartechAuthorizationService> StartechAuthorizationService { get; set; }

        public int ThisUserDatabaseId { get; set; } = 19;

        [Test]
        public async Task for_new_item_a_new_StarpointItem_should_be_created()
        {
            InitializeHttpCallNewItem(ThisUserId);
            var target = CreateComponent(ThisUserId);
            await Task.Delay(30);
            target.Instance.Item.Should().NotBeNull();
        }

        private void InitializeHttpCallNewItem(ComponentParameter userIdParameter)
        {
            int userId = (int)userIdParameter.Value;
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{userId}")
                .RespondValues(new UserObject { Id = userId == ThisUser.Id ? ThisUserDatabaseId : userId, UserName = "User" });
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetStarpointsType/-1")
                .RespondValues(new[] { new StarpointsType { Id = 1 } });
            if(userId != ThisUser.Id)
            {
                MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUserStartechs/{userId}")
                    .RespondValues( new[] { Startechs.Dotnet });
            }
        }

        [Test]
        public async Task for_new_item_created_by_this_user_a_new_StarpointItem_should_be_created_with_current_users_true_id()
        {
            InitializeHttpCallNewItem(ThisUserId);
            var target = CreateComponent(ThisUserId);
            await Task.Delay(30);
            target.Instance.Item.ApplicationUserId.Should().Be(ThisUserDatabaseId);
        }

        [Test]
        public async Task for_new_item_created_by_a_leader_a_new_StarpointItem_should_be_created_with_the_users_item_creators_id()
        {
            InitializeHttpCallNewItem(OtherUserId);
            var target = CreateComponent(OtherUserId);
            await Task.Delay(30);
            target.Instance.Item.ApplicationUserId.Should().Be((int)OtherUserId.Value);
        }

        [Test]
        public async Task for_updated_item_created_by_this_user_the_item_should_be_load_from_server()
        {
            InitializeHttpCallUpdateItem(11, 11);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.Item.Id.Should().Be(107);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        private void InitializeHttpCallUpdateItem(int databaseThisUserId, int applicationUserId)
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetItem/-1/107")
                .RespondValues(new StarpointsItem
                {
                    Id = 107,
                    ApplicationUserId = applicationUserId
                });
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/-1")
                .RespondValues(new UserObject { Id = databaseThisUserId });
            if (databaseThisUserId != applicationUserId)
            {
                MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{applicationUserId}")
                .RespondValues(new UserObject { Id = applicationUserId });
            }
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetStarpointsType/-1")
               .RespondValues(new[] { new StarpointsType { Id = 1 } });
            if (databaseThisUserId != applicationUserId)
            {
                MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUserStartechs/{applicationUserId}")
                    .RespondValues(new[] { Startechs.Dotnet });
            }

        }

        [Test]
        public async Task for_new_item_created_by_this_user_this_current_user_should_be_load()
        {
            InitializeHttpCallUpdateItem(15, 15);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.User.Id.Should().Be(15);
        }

        [Test]
        public async Task for_new_item_created_by_a_leader_the_user_should_be_load()
        {
            InitializeHttpCallUpdateItem(15, 31);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.User.Id.Should().Be(31);
        }

        [Test]
        public async Task for_new_item_created_by_this_user_available_startech_should_be_the_startechs_from_which_user_is_member_of()
        {
            InitializeHttpCallNewItem(ThisUserId);
            MockAuthorizationForAvailableStartechs();
            var target = CreateComponent(ThisUserId);
            await Task.Delay(30);
            target.Instance.AvailableStartechs.Should().BeEquivalentTo(Startechs.Java);
        }

        private void MockAuthorizationForAvailableStartechs()
        {
            StartechAuthorizationService.Setup(x => x.IsMemberOrLeaderOf(Startechs.Java, false)).Returns(Task.Factory.StartNew(() => true));
            foreach(Startechs startech in Enum.GetValues(typeof(Startechs)).Cast<Startechs>().Where(x => x != Startechs.Java))
            {
                StartechAuthorizationService.Setup(x => x.IsMemberOrLeaderOf(startech, false)).Returns(Task.Factory.StartNew(() => false));
            }
        }

        [Test]
        public async Task for_new_item_created_for_another_user_available_startech_should_be_retrieved_from_server()
        {
            InitializeHttpCallNewItem(OtherUserId);
            var target = CreateComponent(OtherUserId);
            await Task.Delay(30);
            target.Instance.AvailableStartechs.Should().BeEquivalentTo(Startechs.Dotnet);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task for_updated_item_created_for_this_user_available_startech_should_be_the_startechs_from_which_user_is_member_of()
        {
            InitializeHttpCallUpdateItem(15, 31);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.User.Id.Should().Be(31);
        }

        [Test]
        public async Task for_updated_item_created_for_another_user_available_startech_should_be_retrieved_from_server()
        {
            InitializeHttpCallUpdateItem(15, 17);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.AvailableStartechs.Should().BeEquivalentTo(Startechs.Dotnet);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        private void MockAuthorizationForStartechsWhichUserIsLeader()
        {
            StartechAuthorizationService.Setup(x => x.IsMemberOrLeaderOf(Startechs.Agile, true)).Returns(Task.Factory.StartNew(() => true));
            foreach (Startechs startech in Enum.GetValues(typeof(Startechs)).Cast<Startechs>().Where(x => x != Startechs.Agile))
            {
                StartechAuthorizationService.Setup(x => x.IsMemberOrLeaderOf(startech, true)).Returns(Task.Factory.StartNew(() => false));
            }
        }


        [Test]
        public async Task startech_current_user_is_leader_of_should_be_load_by_startech_which_current_user_is_leader_of()
        {
            InitializeHttpCallUpdateItem(15, 17);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.StartechWhichCurrentUserIsLeader.Should().BeEquivalentTo(Startechs.Agile);
        }

        [Test]
        public async  Task when_user_isLeader_of_item_startech_isLeader_should_be_true()
        {
            InitializeHttpCallUpdateItem(15, 17);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.StartechWhichCurrentUserIsLeader.Should().BeEquivalentTo(Startechs.Agile);
            target.Instance.Item.Startech = Startechs.Agile;
            target.Instance.IsLeader.Should().BeTrue();
        }

        [Test]
        public async Task when_user_isnot_Leader_of_item_startech_isLeader_should_be_false()
        {
            InitializeHttpCallUpdateItem(15, 17);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.StartechWhichCurrentUserIsLeader.Should().BeEquivalentTo(Startechs.Agile);
            target.Instance.Item.Startech = Startechs.Java;
            target.Instance.IsLeader.Should().BeFalse();
        }

        [Test]
        public async Task when_all_has_been_load_is_load_should_be_true_create_item()
        {
            InitializeHttpCallNewItem(OtherUserId);
            var target = CreateComponent(OtherUserId);
            await Task.Delay(30);
            target.Instance.IsLoad.Should().BeTrue();
        }

        [Test]
        public async Task when_all_has_been_load_is_load_should_be_true_update_item()
        {
            InitializeHttpCallUpdateItem(11, 11);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.IsLoad.Should().BeTrue();
        }

        [Test]
        public async Task for_new_item_isNew_should_be_true()
        {
            InitializeHttpCallNewItem(OtherUserId);
            var target = CreateComponent(OtherUserId);
            await Task.Delay(30);
            target.Instance.IsNew.Should().BeTrue();
        }

        [Test]
        public async Task for_updated_item_isNew_should_be_false()
        {
            InitializeHttpCallUpdateItem(11, 11);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.IsNew.Should().BeFalse();
        }

        [Test]
        public async Task when_creating_an_item_with_this_user_createItem_service_should_be_called_with_this_user_id()
        {
            InitializeHttpCallNewItem(ThisUserId);
            var target = CreateComponent(ThisUserId);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Post, "http://localhost/StarpointsManager/CreateStarpoints/-1").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(ThisUserDatabaseId);
            await target.Instance.UpdateOrCreate();
            MockHttp.VerifyNoOutstandingExpectation();
        }

        private void MockGetThisUserHttpRequest(int userId)
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/User/GetUser/-1")
                 .RespondValues(new UserObject { Id = userId });
        }

        [Test]
        public async Task when_creating_an_item_with_an_other_user_createItem_service_should_be_called_with_the_user_id()
        {
            InitializeHttpCallNewItem(OtherUserId);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(OtherUserId);
            var userId = (int)OtherUserId.Value;
            await Task.Delay(30);
            target.Instance.Item.Startech = Startechs.Agile;
            MockHttp.Expect(HttpMethod.Post, $"http://localhost/StarpointsManager/CreateStarpoints/{userId}").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(userId);
            await target.Instance.UpdateOrCreate();
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_updating_an_item_with_this_user_updateItem_service_should_be_called_with_this_user_id()
        {
            const int userId = 11;
            InitializeHttpCallUpdateItem(userId, userId);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Post, "http://localhost/StarpointsManager/UpdateStarpoints/-1").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(userId);
            await target.Instance.UpdateOrCreate();
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_updating_an_item_with_an_other_user_updateItem_service_should_be_called_with_the_user_id()
        {
            const int userId = 17;
            InitializeHttpCallUpdateItem(11, userId);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.Item.Startech = Startechs.Agile;
            MockHttp.Expect(HttpMethod.Post, $"http://localhost/StarpointsManager/UpdateStarpoints/{userId}").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(userId);
            await target.Instance.UpdateOrCreate();
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_creating_an_item_with_this_user_createItem_service_should_navigate_to_this_users_points()
        {
            InitializeHttpCallNewItem(ThisUserId);
            var target = CreateComponent(ThisUserId);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Post, "http://localhost/StarpointsManager/CreateStarpoints/-1").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(ThisUserDatabaseId);
            await target.Instance.UpdateOrCreate();
            HasNavigatedToUrl.Should().Be("/Points/-1");
        }

        [Test]
        public async Task when_creating_an_item_with_an_other_user_createItem_service_should_navigate_to_the_users_points()
        {
            InitializeHttpCallNewItem(OtherUserId);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(OtherUserId);
            var userId = (int)OtherUserId.Value;
            await Task.Delay(30);
            target.Instance.Item.Startech = Startechs.Agile;
            MockHttp.Expect(HttpMethod.Post, $"http://localhost/StarpointsManager/CreateStarpoints/{userId}").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(33);
            await target.Instance.UpdateOrCreate();
            HasNavigatedToUrl.Should().Be($"/Points/{userId}");
        }

        [Test]
        public async Task when_updating_an_item_with_this_user_updateItem_service_should_navigate_to_this_user_points()
        {
            const int userId = 11;
            InitializeHttpCallUpdateItem(userId, userId);
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Post, $"http://localhost/StarpointsManager/UpdateStarpoints/-1").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(userId);
            await target.Instance.UpdateOrCreate();
            HasNavigatedToUrl.Should().Be("/Points/-1");
        }

        [Test]
        public async Task when_updating_an_item_with_an_other_user_updateItem_service_should_navigate_to_the_users_points()
        {
            const int userId = 17;
            InitializeHttpCallUpdateItem(11, userId);
            MockAuthorizationForStartechsWhichUserIsLeader();
            var target = CreateComponent(TestItemId);
            await Task.Delay(30);
            target.Instance.Item.Startech = Startechs.Agile;
            MockHttp.Expect(HttpMethod.Post, $"http://localhost/StarpointsManager/UpdateStarpoints/{userId}").Respond(HttpStatusCode.OK);
            MockGetThisUserHttpRequest(11);
            await target.Instance.UpdateOrCreate();
            Console.WriteLine(target.Instance.User.Id);
            HasNavigatedToUrl.Should().Be($"/Points/{userId}");
        }
    }
}
