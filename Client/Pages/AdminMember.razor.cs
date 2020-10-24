using Blazor.Startechmanager.Client.Helpers;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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

        [Inject]
        public NavigationManager NavigationManager { get; set; }

#nullable enable

        public List<UserObject> Members { get; set; } = new List<UserObject>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadMembers();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadMembers();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task LoadMembers()
        {
            Members = await HttpClient.GetFromJsonAsync<List<UserObject>>($"AdminMember/{StartechType}/GetMembers");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        public void View(UserObject user)
        {
            NavigationManager.NavigateTo($"Points/{user.Id}");
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
            await HttpClient.DoActionByGetMethod(action, messageDisplayer: MessageDisplayer);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadMembers();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
