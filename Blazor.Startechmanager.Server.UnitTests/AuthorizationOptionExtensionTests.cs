using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class AuthorizationOptionExtensionTests
    {
        public IAuthorizationService AuthorizationService { get; set; }

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAuthorization(options => options.AddAppicationPolicies());
            AuthorizationService = services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        }

        [Test]
        public async Task Admin_policy_requires_Admin_role()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, Roles.Admin) }));
            var authentificationResult = await AuthorizationService.AuthorizeAsync(user, Roles.Admin);
            authentificationResult.Succeeded.Should().BeTrue();
        }
    }
}
