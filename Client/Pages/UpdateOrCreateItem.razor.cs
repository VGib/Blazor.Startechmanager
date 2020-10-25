using Blazor.Startechmanager.Client.Helpers;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Constants;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Pages
{
    public partial class UpdateOrCreateItem
    {
        [Parameter]
        public int UserId { get; set; }
        
        [Parameter]
        public int ItemId { get; set; }

#nullable disable
        [Inject]
        public HttpClient HttpClient { get; set; }

        // authorization: https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda
        [Inject]
        public AuthenticationStateProvider AuthentificationProvider { get; set; }

        [Inject]
        public IAuthorizationService  AuthorizationService { get; set; }

        [Inject]
        public IMessageDisplayer MessageDisplayer  { get; set; }

        [Inject]
        public  NavigationManager NavigationManager { get; set; }

#nullable enable
        public bool IsLoad { get; set; } = false;

        public bool IsNew { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public IList<StarpointsType> ItemTypes { get; set; }

        public UserObject User { get; set; } 

        public StarpointsItem  Item { get; set; }

        public IList<Startechs> AvailableStartechs { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public bool IsLeader
        {
            get
            {
                return IsMemberOrLeaderOf(Item?.Startech ?? Startechs.Agile).GetAwaiter().GetResult();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (UserId != default)
            {
                IsNew = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                LoadForNewItem(UserId);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else if (ItemId != default)
            {
                IsNew = false;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                LoadForUpdateItem(ItemId);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else {
                throw new Exception("should not append, this is  a bug, please report it");
            }
        }

        private async Task LoadForNewItem(int userId)
        {
            Item = new StarpointsItem()
            {
                ValidationState = ValidationState.InStudy
            };
            User = await GetUser(userId);
            Item.ApplicationUserId = User.Id;
            await LoadItemTypes();
            await LoadUsersStartechs();
            IsLoad = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task LoadUsersStartechs()
        {
            if(UserId == ThisUser.Id)
            {
                AvailableStartechs = Enum.GetValues(typeof(Startechs)).Cast<Startechs>().Where(x => IsMemberOrLeaderOf(x).GetAwaiter().GetResult()).ToArray();
            }
            else
            {
                AvailableStartechs = await HttpClient.GetFromJsonAsync<IList<Startechs>>($"User/GetUserStartechs/{User.Id}");
            }
        }

        private  Task<UserObject> GetUser(int userId)
        {
            return HttpClient.GetFromJsonAsync<UserObject>($"User/GetUser/{userId}");
        }

        private async Task LoadForUpdateItem(int itemId)
        {
            Item = await HttpClient.GetFromJsonAsync<StarpointsItem>($"StarpointsManager/GetItem/-1/{itemId}");
            User = await GetUser(Item.ApplicationUserId);
            await LoadItemTypes();
            IsLoad = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task LoadItemTypes()
        {
            ItemTypes = await HttpClient.GetFromJsonAsync<IList<StarpointsType>>("StarpointsManager/GetStarpointsType/-1");
        }

        private async Task<bool> IsMemberOrLeaderOf(Startechs startech, bool isLeader = true)
        {
            if(!Enum.IsDefined(typeof(Startechs), startech))
            {
                return false;
            }

            var authentificationState = await AuthentificationProvider.GetAuthenticationStateAsync();
            return (await AuthorizationService.AuthorizeAsync(authentificationState.User, StartechPolicyHelper.GetPolicyName(startech, isLeader))).Succeeded;
        }

        public void OnTypeChanged(ChangeEventArgs args)
        {
            int typeIdFromArgsValue = int.Parse(args.Value as string);
            Item.Type = ItemTypes.FirstOrDefault(x => x.Id == typeIdFromArgsValue);
            Item.NumberOfPoints = Item.Type?.NumberOfPoint ?? 0;
        }

        public async void UpdateOrCreate()
        {
            if(IsNew)
            {
                await HttpClient.DoActionByPost($"StarpointsManager/CreateStarpoints/{UserId}", Item, MessageDisplayer);
            }
            else
            {
                await HttpClient.DoActionByPost($"StarpointsManager/UpdateStarpoints/{UserId}", Item, MessageDisplayer);
            }
            ReturnToStarpointItemsList();    
        }
        
        public void ReturnToStarpointItemsList()
        {
            var userIdToNavigate = IsLeader ? User.Id : ThisUser.Id;
            NavigationManager.NavigateTo($"/Points/{ userIdToNavigate }");
        }
    }
}
