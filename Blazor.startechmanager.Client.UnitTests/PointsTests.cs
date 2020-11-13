using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Shared.Constants;
using Bunit;
using System.Text.Json;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Startechmanager.Shared.Models;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using Blazor.Startechmanager.Client.Services;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class PointsTests : BaseTestsForComponent<Points>
    {
        public Mock<IStartechAuthorizationService> StartechAuthorizationService { get; set; }

        public Mock<IMessageDisplayer> MessageDisplayer { get; set; }

        public Mock<IConfirmDisplayer> ConfirmDisplayer { get; set; }

        public ComponentParameter ThisUserIdParameter { get; } = ComponentParameterFactory.Parameter("UserId", 0);

        public ComponentParameter OtherUserIdParameter { get; } = ComponentParameterFactory.Parameter("UserId", 17);

        [Test]
        public async Task when_creating_an_item_the_user_should_navigate_to_createItem_page()
        {
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
 .Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(ThisUserIdParameter);
            await Task.Delay(50);
            target.Instance.CreateItem();
            HasNavigatedToUrl.Should().Be("/NewItem/-1");
        }

        [Test]
        public async Task when_creating_an_item_the_user_should_navigate_to_createItem_page_with_userId_information()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/User/GetUser/17")
.Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 17, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetStarpoints/17")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(OtherUserIdParameter);
            await Task.Delay(50);
            target.Instance.CreateItem();
            HasNavigatedToUrl.Should().Be("/NewItem/17");
        }

        [Test]
        public async Task the_component_should_load_users_ids_items()
        {
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
    .Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(ThisUserIdParameter);
            await Task.Delay(50);
            target.Instance.Items.Should().HaveCount(1);
        }

        [Test]
        public async Task the_component_should_load_users_ids_userObject()
        {
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
 .Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(ThisUserIdParameter);
            await Task.Delay(50);
            target.Instance.User.Id.Should().Be(1);
        }

        [Test]
        public async Task when_the_item_have_been_load_isLoad_should_be_true()
        {
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
.Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(ThisUserIdParameter);
            await Task.Delay(50);
            target.Instance.IsLoad.Should().BeTrue();
        }

        [Test]
        public async Task if_user_id_is_0_user_should_be_thisUser()
        {
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
.Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(ThisUserIdParameter);
            await Task.Delay(50);
            target.Instance.UserId.Should().Be(ThisUser.Id);
        }

        [Test]
        public async Task if_user_id_is_strictly_positive_user_should_be_the_user_of_the_user_id()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/User/GetUser/17")
.Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 17, NumberOfpoints = 17 }));
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetStarpoints/17")
                .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem() }));
            var target = CreateComponent(OtherUserIdParameter);
            await Task.Delay(50);
            target.Instance.UserId.Should().Be(17);
        }
    }
}
