using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Pages
{
    public partial class AdminMember
    {
#nullable disable
        [Parameter]
        public string StartechType { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IMessageDisplayer MessageDisplayer { get; set; }
#nullable enable

        public List<UserObject> Members { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadMembers();
        }

        public async Task LoadMembers()
        {
            Members = await HttpClient.GetFromJsonAsync<List<UserObject>>($"AdminMember/{StartechType}/GetMembers");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        public async Task View(UserObject user)
        {
            throw new NotImplementedException("to do");
        }

        public async Task Remove(UserObject user)
        {
            await DoAction($"AdminMember/{StartechType}/RemoveMember/{user.Id}");
        }

        public async Task Add(UserObject user)
        {
            await DoAction($"AdminMember/{StartechType}/SetMember/{user.Id}");
        }

        private async Task DoAction(string action)
        {
            var result = await HttpClient.GetAsync(action);
            if (!result?.IsSuccessStatusCode ?? false)
            {
                await MessageDisplayer.DisplayErrorMessage(await result.Content.ReadAsStringAsync());
            }
            LoadMembers();
        }
    }
}
