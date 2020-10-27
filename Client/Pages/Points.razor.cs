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
        [Parameter]
        public int UserId { get; set; } = ThisUser.Id;

        public bool IsLoad { get; set; } = false;

        public List<StarpointsItem> Items = new List<StarpointsItem>();

        public UserObject User { get; set; }


        [Inject]
        public HttpClient HttpClient { get; set; }

        public IList<Startechs> UserIsLeaderOfStartechs { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if(UserId == default(int))
            {
                UserId = ThisUser.Id;
            }

            Load();
        }

        public async Task Load ()
        {
            User = await HttpClient.GetFromJsonAsync<UserObject>($"User/GetUser/{UserId}");
            Items = await HttpClient.GetFromJsonAsync<List<StarpointsItem>>($"StarpointsManager/GetStarpoints/{UserId}");

            IsLoad = true;
            InvokeAsync(StateHasChanged);
        }
    }
}
