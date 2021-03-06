﻿using Blazor.Startechmanager.Client.Helpers;
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
        public List<UserObject> Leaders { get; set; } = new List<UserObject>();

        public async Task OnRemove(UserObject user)
        {
            await DoAction($"AdminLeader/{StartechType}/RemoveLeader/{user.Id}");
        }

        private async Task DoAction(string action)
        {
            await HttpClient.DoActionByGetMethod(action, messageDisplayer: MessageDisplayer);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadClients();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
                Leaders = await HttpClient.GetFromJsonAsync<List<UserObject>>($"AdminLeader/{StartechType}/GetLeaders");
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

        public async Task AddUser(UserObject user)
        {
            await DoAction($"AdminLeader/{StartechType}/AddLeader/{user.Id}");
        }
    }
}
