using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Shared.Policies
{
    public static class AuthorizationOptionsExtension
    {
        public static AuthorizationOptions AddAppicationPolicies(this AuthorizationOptions option)
        {
            option.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));

            foreach (Startechs startech in Enum.GetValues(typeof(Startechs)))
            {
                option.AddPolicy(StartechPolicyHelper.GetPolicyName(startech, MustBeLeader: false)
                    , policy => policy.AddRequirements(new StartechPolicyAuthorizationRequirement { StartechType = startech, IsLeader = false }));
                option.AddPolicy(StartechPolicyHelper.GetPolicyName(startech, MustBeLeader: true)
                    , policy => policy.AddRequirements(new StartechPolicyAuthorizationRequirement { StartechType = startech, IsLeader = true }));
            }

            option.AddPolicy(StartechPolicyHelper.AllStartechLeader, policy => policy.AddRequirements(new AllStartechPolicyAuthorizationRequiement { IsLeader = true }));
            option.AddPolicy(StartechPolicyHelper.AllStartechMember, policy => policy.AddRequirements(new AllStartechPolicyAuthorizationRequiement { IsLeader = false }));

            return option;
        }
    }

    public static class ConfigueStartechPoliciesServicesExtension
    {
        public static IServiceCollection AddStartechPoliciesHandler(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, StartechPolicyAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AllStartechPolicyAuthorizationHandler>();

            return services;
        }
    }

    public static class StartechPolicyHelper
    {
        public static string GetPolicyName(Startechs startech, bool MustBeLeader = false)
        {
            return $"Startech::{startech}::{(MustBeLeader ? StartechClaimHelper.Leader : StartechClaimHelper.Member) }"; 
        }

        public const string AllStartechLeader = "AllStartechLeader";

        public const string AllStartechMember = "AllStartechMemeber";
    }

    public static class StartechClaimHelper
    {
        public static string GetStartechClaimType(Startechs startech)
        {
            return $"StartechMember::{startech}";
        }

        public const string Member = "Member";

        public const string Leader = "Leader";
    }

    public class StartechPolicyAuthorizationRequirement : IAuthorizationRequirement
    {
        public Startechs StartechType { get; set; }

        public bool IsLeader { get; set; }
    }

    public class AllStartechPolicyAuthorizationRequiement : IAuthorizationRequirement
    {
        public bool IsLeader { get; set; }


    }

    public class StartechPolicyAuthorizationHandler : AuthorizationHandler<StartechPolicyAuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StartechPolicyAuthorizationRequirement requirement)
        {
            if (context.User.IsInRole(Roles.Admin))
            {
                context.Succeed(requirement);
                return;
            }

            var startechClaim = context.User.Claims.FirstOrDefault(x => x.Type == StartechClaimHelper.GetStartechClaimType(requirement.StartechType));
            if (startechClaim != null && !requirement.IsLeader)
            {
                context.Succeed(requirement);
            }
            else if (startechClaim != null && startechClaim.Value == StartechClaimHelper.Leader)
            {
                context.Succeed(requirement);
            }
        }
    }

    public class AllStartechPolicyAuthorizationHandler : AuthorizationHandler<AllStartechPolicyAuthorizationRequiement>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllStartechPolicyAuthorizationRequiement requirement)
        {
            if (context.User.IsInRole(Roles.Admin))
            {
                context.Succeed(requirement);
                return;
            }

            if(context.User.Claims.Any(x => x.Type.StartsWith("StartechMember::") && (!requirement.IsLeader || x.Value == StartechClaimHelper.Leader)))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
