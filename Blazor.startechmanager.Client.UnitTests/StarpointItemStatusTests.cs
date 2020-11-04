using Blazor.Startechmanager.Client.Component;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class StarpointItemStatusTests : BaseTestsForComponent<StarpointItemStatus>
    {
        public Mock<IStartechAuthorizationService> StartechAuthorizationService { get; set; }

        public Mock<IMessageDisplayer> MessageDisplayer { get; set; }

        public Mock<IConfirmDisplayer> ConfirmDisplayer { get; set; }



        private StarpointsItem CreateStarpointsItem()
        {
            return new StarpointsItem
            {
                Id = 17,
                Startech = Startechs.Dotnet,
                ValidationState = ValidationState.InStudy
            };
        }

        private void Authorize(StarpointsItem starpointsItem,  bool isLeader = false)
        {
            var isLeaderValue = isLeader;
            var itemsStartech = starpointsItem.Startech;
            StartechAuthorizationService.Setup(x => x.IsMemberOrLeaderOf(It.IsIn(itemsStartech), It.IsIn(true)))
                .Returns(Task.Factory.StartNew(() => isLeader));
        }

        [Test]
        public async Task isleader_should_be_true_if_user_is_leader_of_current_item_startech()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, true);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            component.Instance.IsLeaderOfItemStartech.Should().BeTrue();
        }

        [Test]
        public async Task isleader_should_be_false_if_user_is_not_leader_of_current_item_startech()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            component.Instance.IsLeaderOfItemStartech.Should().BeFalse();
        }


        [Test]
        public async Task when_suppressing_item_if_the_pop_up_retrieve_no_the_item_should_not_be_suppressed()
        {
            ConfirmDisplayer.Setup(x => x.Confirm(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => false));
            bool hasCallRemoveCallback = false;
            var removeItemEventCallBack =EventCallbackFactory.Create<StarpointsItem>(new object(), _ => hasCallRemoveCallback = true);
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem)
                , ComponentParameter.CreateParameter("PleaseRemoveItem",removeItemEventCallBack));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.CancelItem();
            await Task.Delay(30);
            hasCallRemoveCallback.Should().BeFalse();
        }

        [Test]
        public async Task when_suppressing_item_if_the_pop_up_retrieve_yes_the_item_should_be_suppressed()
        {
            ConfirmDisplayer.Setup(x => x.Confirm(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => true));
            bool hasCallRemoveCallback = false;
            var removeItemEventCallBack = EventCallbackFactory.Create<StarpointsItem>(new object(), _ => hasCallRemoveCallback = true);
            var starpointsItem = CreateStarpointsItem();
            MockHttp.Expect($"http://localhost/StarpointsManager/CancelStarpoints/-1/{starpointsItem.Id}").Respond(HttpStatusCode.OK);
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem)
                , ComponentParameter.CreateParameter("PleaseRemoveItem", removeItemEventCallBack));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.CancelItem();
            await Task.Delay(30);
            hasCallRemoveCallback.Should().BeTrue();
        }

        [Test]
        public async Task when_suppressing_item_the_suppress_event_call_back_should_be_called()
        {
            ConfirmDisplayer.Setup(x => x.Confirm(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => true));
            var starpointsItem = CreateStarpointsItem();
            bool hasCallRemoveCallback = false;
            var removeItemEventCallBack = EventCallbackFactory.Create<StarpointsItem>(new object(), item => { if (item == starpointsItem) { hasCallRemoveCallback = true; } });
            MockHttp.Expect($"http://localhost/StarpointsManager/CancelStarpoints/-1/{starpointsItem.Id}").Respond(HttpStatusCode.OK);
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem)
                , ComponentParameter.CreateParameter("PleaseRemoveItem", removeItemEventCallBack));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.CancelItem();
            await Task.Delay(30);
            hasCallRemoveCallback.Should().BeTrue();
        }

        [Test]
        public async Task when_updating_status_the_update_status_api_method_should_be_called()
        {
            ConfirmDisplayer.Setup(x => x.Confirm(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => true));
            var removeItemEventCallBack = EventCallbackFactory.Create<StarpointsItem>(new object(), _ => { });
            var starpointsItem = CreateStarpointsItem();
            MockHttp.Expect($"http://localhost/StarpointsManager/CancelStarpoints/-1/{starpointsItem.Id}").Respond(HttpStatusCode.OK);
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem)
                , ComponentParameter.CreateParameter("PleaseRemoveItem", removeItemEventCallBack));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.CancelItem();
            await Task.Delay(30);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_updating_status_the_update_status_api_method_should_be_called_for_this_item()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            MockHttp.Expect($"http://localhost/StarpointsManager/UpdateValidationStatus/-1/{starpointsItem.Id}/{ValidationState.Validated}").Respond(HttpStatusCode.OK);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.UpdateState(ValidationState.Validated);
            MockHttp.VerifyNoOutstandingExpectation();

        }

        [Test]
        public async Task when_updating_status_the_update_status_api_method_should_be_called_for_new_status()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            MockHttp.Expect($"http://localhost/StarpointsManager/UpdateValidationStatus/-1/{starpointsItem.Id}/{ValidationState.Validated}").Respond(HttpStatusCode.OK);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.UpdateState(ValidationState.Validated);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_updating_status_the_status_should_be_changed_in_component()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            MockHttp.Expect($"http://localhost/StarpointsManager/UpdateValidationStatus/-1/{starpointsItem.Id}/{ValidationState.Validated}").Respond(HttpStatusCode.OK);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            await component.Instance.UpdateState(ValidationState.Validated);
            starpointsItem.ValidationState.Should().Be(ValidationState.Validated);
        }

        [Test]
        public async Task when_updating_item_the_application_should_navigate_to_UpdateItem_page()
        {
            var starpointsItem = CreateStarpointsItem();
            Authorize(starpointsItem, false);
            var component = CreateComponent(ComponentParameter.CreateParameter("Item", starpointsItem));
            component.WaitForState(() => component.Nodes?.Length > 0);
            component.Instance.UpdateItem();
            HasNavigatedToUrl.Should().Be($"/UpdateItem/{starpointsItem.Id}");
        }
    }
}