﻿using Blazor.Startechmanager.Client.Helpers;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Constants;
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
    public partial class UpdateOrCreateItem
    {
        [Parameter]
        public int UserId { get; set; }
        
        [Parameter]
        public int ItemId { get; set; }

#nullable disable
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IMessageDisplayer MessageDisplayer  { get; set; }

        [Inject]
        public  NavigationManager NavigationManager { get; set; }

        [Inject]
        public IStartechAuthorizationService StartechAuthorizationService { get; set; }

#nullable enable
        public bool IsLoad { get; set; } = false;

        public bool IsNew { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public IList<StarpointsType> ItemTypes { get; set; }

        public UserObject User { get; set; } 

        public StarpointsItem  Item { get; set; }

        public IList<Startechs> AvailableStartechs { get; set; }

        public IList<Startechs> StartechWhichCurrentUserIsLeader { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public bool IsLeader
        {
            get
            {
                return StartechWhichCurrentUserIsLeader.Any(x => Item.Startech == x);
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
                AvailableStartechs = await GetStartechWhichMemberOrLeader(false);
            }
            else
            {
                AvailableStartechs = await HttpClient.GetFromJsonAsync<IList<Startechs>>($"User/GetUserStartechs/{User.Id}");
            }

            StartechWhichCurrentUserIsLeader = await GetStartechWhichMemberOrLeader(true);
        }

        private async Task<IList<Startechs>> GetStartechWhichMemberOrLeader(bool isLeader)
        {
            var startechs = new List<Startechs>();
            foreach (Startechs startech in Enum.GetValues(typeof(Startechs))) if (await StartechAuthorizationService.IsMemberOrLeaderOf(startech, isLeader)) startechs.Add(startech);
            return startechs;
        }

        private  Task<UserObject> GetUser(int userId)
        {
            return HttpClient.GetFromJsonAsync<UserObject>($"User/GetUser/{userId}");
        }

        private async Task LoadForUpdateItem(int itemId)
        {
            Item = await HttpClient.GetFromJsonAsync<StarpointsItem>($"StarpointsManager/GetItem/-1/{itemId}");
            var thisUser = await GetUser(ThisUser.Id);
            if (thisUser.Id == Item.ApplicationUserId)
            {
                UserId = ThisUser.Id;
                User = thisUser;
            }
            else
            {
                User = await GetUser(Item.ApplicationUserId);
            }
            await LoadItemTypes();
            await LoadUsersStartechs();
            IsLoad = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InvokeAsync(StateHasChanged);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task LoadItemTypes()
        {
            ItemTypes = await HttpClient.GetFromJsonAsync<IList<StarpointsType>>("StarpointsManager/GetStarpointsType/-1");
        }

        public void OnTypeChanged(ChangeEventArgs args)
        {
            int typeIdFromArgsValue = int.Parse(args.Value as string);
            Item.Type = ItemTypes.FirstOrDefault(x => x.Id == typeIdFromArgsValue);
            Item.NumberOfPoints = Item.Type?.NumberOfPoint ?? 0;
        }

        public async Task UpdateOrCreate()
        {
            var userId = IsLeader ? User.Id : ThisUser.Id;
            if(IsNew)
            {
                await HttpClient.DoActionByPost($"StarpointsManager/CreateStarpoints/{userId}", Item, MessageDisplayer);
            }
            else
            {
                await HttpClient.DoActionByPost($"StarpointsManager/UpdateStarpoints/{userId}", Item, MessageDisplayer);
            }
            await ReturnToStarpointItemsList();    
        }
        
        public async Task ReturnToStarpointItemsList()
        {
            var userIdToNavigate = (await  IsThisUser()) ? ThisUser.Id : User.Id;
            NavigationManager.NavigateTo($"/Points/{ userIdToNavigate }");
        }

        private async Task<bool> IsThisUser()
        {
            var thisUser = await HttpClient.GetFromJsonAsync<UserObject>("User/GetUser/-1");
            return thisUser.Id == User.Id;
        }
    }
}
