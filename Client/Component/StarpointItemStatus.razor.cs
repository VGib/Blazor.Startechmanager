using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Component
{
    public partial class StarpointItemStatus
    {
#nullable disable
        [Parameter]
        public StarpointsItem   Item { get; set; }

        // authorization: https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda
        [Inject]
        public IStartechAuthorizationService StartechAuthorizationService { get; set; }
#nullable enable

        public bool IsLeaderOfItemStartech { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            IsLeaderOfItemStartech = await StartechAuthorizationService.IsMemberOrLeaderOf(Item.Startech, true);
        }
    }
}
