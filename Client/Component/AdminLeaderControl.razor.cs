using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        [Inject]
        public IMessageDisplayer  MessageDisplayer { get; set; }

#nullable enable

        public UserObject? UserObjectToAdd { get; set; }

        public List<UserObject> Leaders { get; set; } = new List<UserObject>();

        public async Task OnRemove(UserObject user)
        {
            await DoAction($"StartechLeader/{StartechType}/RemoveLeader/{user.Id}");
        }

        private async Task DoAction(string action)
        {
            var result = await HttpClient.GetAsync(action);
            if (!result?.IsSuccessStatusCode ?? false)
            {
                await MessageDisplayer.Display("something occurs", $"an error occured: {await result.Content.ReadAsStringAsync()}");
            }
            LoadClients();
        }

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

        public async Task<IEnumerable<UserObject>> SearchUser(string searchText)
        {
            var result = await HttpClient.PostAsJsonAsync<string>("SearchUser", searchText);
            if(!result.IsSuccessStatusCode)
            {
                return Array.Empty<UserObject>();
            }

            return await result.Content.ReadFromJsonAsync<IList<UserObject>>();
        }

        public async Task AddUser()
        {
            if ( UserObjectToAdd is null)
            {
                return;
            }
            await DoAction($"StartechLeader/{StartechType}/AddLeader/{UserObjectToAdd.Id}");
        }
    }
}
