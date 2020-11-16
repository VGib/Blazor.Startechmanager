
using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using Bunit.Rendering;
using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.AspNetCore.Components;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class AdminMemberTests : BaseTestsForComponent<AdminMember>
    {
        public Mock<IMessageDisplayer> MessageDisplayer { get; set; }

        public Mock<NavigationManager> NavigationManager { get; set; }

        public ComponentParameter StartechType { get; set; } = ComponentParameterFactory.Parameter("StartechType", "Dotnet");

        [SetUp]
        public void SetupThisTest()
        {
            ServiceCollection.AddLogging();
        }

        [Test]
        public async Task should_load_startech_members_from_right_startech()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                    .RespondValues(new[] { new UserObject { Id = 12, UserName = "dotnet_Member" } });
            var target = CreateComponent(StartechType);
            await Task.Delay(30);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task should_load_startech_members_at_initialization()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
             .RespondValues(new[] { new UserObject { Id = 12, UserName = "dotnet_Member" } });
            var target = CreateComponent(StartechType);
            target.WaitForAssertion(() => target.Nodes.Any(x => x.NodeValue?.Contains("dotnet_Member") ?? false));
        }

        [Test]
        public async Task when_removing_a_user_the_user_should_be_removed()
        {
            var userObjectToRemove = new UserObject { Id = 12, UserName = "dotnet_Member" };
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                .RespondValues(new[] { userObjectToRemove});
            var target = CreateComponent(StartechType);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/RemoveMember/12")
 .Respond(HttpStatusCode.OK);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
 .RespondValues(new object[0]);
            await target.Instance.Remove(userObjectToRemove);
            MockHttp.VerifyNoOutstandingExpectation();

        }

        [Test]
        public async Task when_adding_a_user_the_user_should_be_added()
        {
            var userObjectToAdd = new UserObject { Id = 12, UserName = "dotnet_Member" };
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                .RespondValues(new object [0]);
            var target = CreateComponent(StartechType);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/SetMember/12")
                    .Respond(HttpStatusCode.OK);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                     .RespondValues(new[] { userObjectToAdd });
            await target.Instance.Add(userObjectToAdd);
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_adding_a_user_the_members_should_be_reload()
        {
            var userObjectToAdd = new UserObject { Id = 12, UserName = "dotnet_Member" };
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                .RespondValues(new object[0]);
            var target = CreateComponent(StartechType);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/SetMember/12")
 .Respond(HttpStatusCode.OK);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
 .RespondValues(new[] { userObjectToAdd });
            await target.Instance.Add(userObjectToAdd);
            target.WaitForAssertion(() => target.Nodes.Any(x => x.NodeValue?.Contains("dotnet_Member") ?? false));
        }

        [Test]
        public async Task after_removing_a_user_the_members_should_be_reload()
        {
            var userObjectToRemove = new UserObject { Id = 12, UserName = "dotnet_Member" };
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
                .RespondValues(new[] { userObjectToRemove });
            var target = CreateComponent(StartechType);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/RemoveMember/12")
 .Respond(HttpStatusCode.OK);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/AdminMember/Dotnet/GetMembers")
             .RespondValues(new object[0]);
            await target.Instance.Remove(userObjectToRemove);
            target.WaitForAssertion(() => target.Nodes.All(x => (x.NodeValue?.Contains("dotnet_Member") ?? false)));
        }
    }
}
