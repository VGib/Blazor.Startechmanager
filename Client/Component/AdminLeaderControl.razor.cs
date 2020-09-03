using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Component
{
    public partial class AdminLeaderControl
    {
#nullable disable
        [Parameter]
        public string DisplayName { get; set; }

        [Parameter]
        public string StartechType { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }
#nullable enable

        public List<UserObject> Leaders { get; set; } = new List<UserObject>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadClients();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task LoadClients()
        {
                Leaders = await HttpClient.GetFromJsonAsync<List<UserObject>>($"StartechLeader/{StartechType}/GetLeaders");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }
    }
}
