using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blazor.Startechmanager.Shared.Policies
{
    public static class AuthorizationOptionsExtension
    {
        public static AuthorizationOptions AddAppicationPolicies(this AuthorizationOptions option)
        {
            option.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));
            return option;
        }
    }
}
