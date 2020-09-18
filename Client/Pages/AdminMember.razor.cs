using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
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
#nullable enable

        public List<UserObject> Users { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadUsers();
        }

        public async Task LoadUsers()
        {
            Users = await HttpClient.GetFromJsonAsync<List<UserObject>>($"AdminMember/{StartechType}/GetMembers");
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
            throw new NotImplementedException("to do");
        }
    }
}
