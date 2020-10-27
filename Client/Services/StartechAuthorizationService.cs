using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public class StartechAuthorizationService : IStartechAuthorizationService
    {
        public StartechAuthorizationService(AuthenticationStateProvider authentificationProvider, IAuthorizationService authorizationService)
        {
            this.authentificationProvider = authentificationProvider;
            this.authorizationService = authorizationService;
        }

        // authorization: https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda
        private readonly AuthenticationStateProvider authentificationProvider;

        private readonly IAuthorizationService authorizationService;

        public async Task<bool> IsMemberOrLeaderOf(Startechs startech, bool mustBeLeader = true)
        {
            if (!Enum.IsDefined(typeof(Startechs), startech))
            {
                return false;
            }

            var authentificationState = await authentificationProvider.GetAuthenticationStateAsync();
            return (await authorizationService.AuthorizeAsync(authentificationState.User, StartechPolicyHelper.GetPolicyName(startech, mustBeLeader))).Succeeded;
        }
    }
}
