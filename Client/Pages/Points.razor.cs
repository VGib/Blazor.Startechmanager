using Blazor.Startechmanager.Shared.Constants;
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
    public partial class Points
    {
#nullable disable
        [Parameter]
        public int UserId { get; set; } = ThisUser.Id;

        public UserObject User { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }
#nullable enable

        public bool IsLoad { get; set; } = false;

        public List<StarpointsItem> Items = new List<StarpointsItem>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if(UserId == default)
            {
                UserId = ThisUser.Id;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Load();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task Load ()
        {
            User = await HttpClient.GetFromJsonAsync<UserObject>($"User/GetUser/{UserId}");
            Items = await HttpClient.GetFromJsonAsync<List<StarpointsItem>>($"StarpointsManager/GetStarpoints/{UserId}");

            IsLoad = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
