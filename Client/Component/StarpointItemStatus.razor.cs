using Blazor.Startechmanager.Client.Helpers;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Component
{
    public partial class StarpointItemStatus
    {
#nullable disable
        [Parameter]
        public StarpointsItem   Item { get; set; }

        [Parameter]
        public EventCallback<StarpointsItem> PleaseRemoveItem { get; set; }

        // authorization: https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda
        [Inject]
        public IStartechAuthorizationService StartechAuthorizationService { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IMessageDisplayer MessageDisplayer { get; set; }

        [Inject]
        public IConfirmDisplayer ConfirmDisplayer { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

#nullable enable

        public bool IsLeaderOfItemStartech { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            IsLeaderOfItemStartech = await StartechAuthorizationService.IsMemberOrLeaderOf(Item.Startech, true);
        }

        public async Task UpdateState(ValidationState state)
        {
            var result = await HttpClient.DoActionByGetMethod($"StarpointsManager/UpdateValidationStatus/-1/{Item.Id}/{state}", MessageDisplayer);
            if(result == ActionStatus.Done)
            {
                Item.ValidationState = state;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public async Task CancelItem()
        {
            if (await ConfirmDisplayer.Confirm("Are you sure ?", "you will suppress this item, are you sure?"))
            {
                var result = await HttpClient.DoActionByGetMethod($"StarpointsManager/CancelStarpoints/-1/{Item.Id}", MessageDisplayer);
                if (result == ActionStatus.Done)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    PleaseRemoveItem.InvokeAsync(Item);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }

        public void UpdateItem()
        {
            NavigationManager.NavigateTo($"/UpdateItem/{Item.Id}");
        }
    }
}
