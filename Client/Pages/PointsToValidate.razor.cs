using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Pages
{
    public partial class PointsToValidate
    {
#nullable disable
        [Inject]
        public HttpClient HttpClient { get; set; }
#nullable enable

        public List<StarpointsItem> Items = new List<StarpointsItem>();

        public bool IsLoad { get; set; } = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Load();
        }

            public async Task Load()
        {
            Items = await HttpClient.GetFromJsonAsync<List<StarpointsItem>>($"StarpointsManager/GetInValidationStarpoints");

            IsLoad = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
