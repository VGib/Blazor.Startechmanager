using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Shared.Constants;
using Bunit;
using Bunit.Rendering;
using System.Text.Json;
using RichardSzalay.MockHttp;
using System;
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

        public ComponentParameter UserIdParameter { get; } = ComponentParameterFactory.Parameter("UserId", 0);

        [Test]
        public async Task when_creating_an_item_the_user_should_navigate_to_createItem_page()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_creating_an_item_the_user_should_navigate_to_createItem_page_with_userId_information()
        {
            throw new NotImplementedException("to do");
        }

    //    [Test]
    //    public async Task the_component_should_load_users_ids_items()
    //    {
    //        MockHttp.Expect(HttpMethod.Get, $"http://localhost/User/GetUser/{ThisUser.Id}")
    //.Respond("application/json", JsonSerializer.Serialize(new UserObject { Id = 1, NumberOfpoints = 17 }));
    //        MockHttp.Expect(HttpMethod.Get, $"http://localhost/StarpointsManager/GetStarpoints/{ThisUser.Id}")
    //            .Respond("application/json", JsonSerializer.Serialize(new[] { new StarpointsItem( ) }));
    //        var target = CreateComponent(UserIdParameter);
    //        target.WaitForState(() => target.Instance.IsLoad);
    //        target.Instance.Items.Should().HaveCount(1);
            
    //    }

        [Test]
        public async Task the_component_should_load_users_ids_userObject()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task when_the_item_have_been_load_isLoad_should_be_true()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task if_user_id_is_0_user_should_be_thisUser()
        {
            throw new NotImplementedException("to do");
        }
        [Test]
        public async Task if_user_id_is_strictly_positive_user_should_be_the_user_of_the_user_id()
        {
            throw new NotImplementedException("to do");
        }

    }
}
