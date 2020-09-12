using AngleSharp.Dom;
using Blazor.Startechmanager.Client.Component;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using Bunit.Rendering;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{

    public class AdminLeaderControlTests : BaseTestsForComponent<AdminLeaderControl>
    { 

        public ComponentParameter StartechType { get; } = ComponentParameterFactory.Parameter(nameof(AdminLeaderControl.StartechType), "Admin");

        public ComponentParameter DisplayName { get; } = ComponentParameterFactory.Parameter(nameof(AdminLeaderControl.DisplayName), "Administrator");

        public IEnumerable<UserObject> Users { get; } = new[] { new UserObject { UserName = "Admin", Id =  1 } };

        public Mock<IMessageDisplayer>  MessageDisplayer { get; set; }


        [Test]
        public async Task when_loading_leaders_the_razor_component_should_be_refreshed_with_leader()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders")
                .Respond("application/json", JsonSerializer.Serialize(new [] { new UserObject { UserName = "jsqdklqdqs" } }));
            var target = CreateComponent(StartechType, DisplayName);
            target.WaitForAssertion( () => target.Nodes.QuerySelectorAll("li:contains(\"jsqdklqdqs\")").Any(), TimeSpan.FromMilliseconds(30));
        }

        [Test]
        public async Task should_load_the_leaders_from_the_right_startechType_api()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(new UserObject[0]));
            var target = CreateComponent(StartechType, DisplayName);
            await Task.Delay(30);
            MockHttp.VerifyNoOutstandingExpectation();
           
        }

        [Test]
        public async Task when_adding_a_leader_the_right_startechtype_api_should_be_called()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(Users));
            var target = CreateComponent(StartechType, DisplayName);
            await Task.Delay(30);
            var userObjectToAdd = new UserObject { Id = 5,  UserName =  "toAdd13215" };
            target.Instance.UserObjectToAdd = userObjectToAdd;
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/AddLeader/5").Respond(HttpStatusCode.OK);
            target.Instance.AddUser();
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_removing_a_leader_the_right_startechtype_api_should_be_called()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(Users));
            var target = CreateComponent(StartechType, DisplayName);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/RemoveLeader/1").Respond(HttpStatusCode.OK);
            target.Instance.OnRemove(Users.First());
            MockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task when_adding_a_leader_the_leader_list_should_be_reload()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(Users));
            var target = CreateComponent(StartechType, DisplayName);
            await Task.Delay(30);
            var userObjectToAdd = new UserObject { Id = 5, UserName = "toAdd13215" };
            target.Instance.UserObjectToAdd = userObjectToAdd;
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/AddLeader/5").Respond(HttpStatusCode.OK);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(Users.Union(new[] { userObjectToAdd})));
            target.Instance.AddUser();
            target.WaitForAssertion(() => target.Nodes.QuerySelectorAll("li:contains(\"toAdd13215\")").Empty(), TimeSpan.FromMilliseconds(30));
        }

        [Test]
        public async Task when_removing_a_leader_the_leader_list_should_be_called()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(Users));
            var target = CreateComponent(StartechType, DisplayName);
            await Task.Delay(30);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/RemoveLeader/1").Respond(HttpStatusCode.OK);
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StartechLeader/Admin/GetLeaders").Respond("application/json", JsonSerializer.Serialize(new UserObject[0]));
            target.Instance.OnRemove(Users.First());
            target.WaitForAssertion(() => target.Nodes.QuerySelectorAll("li:contains(\"Admin\")").Empty(), TimeSpan.FromMilliseconds(30));
        }
    }
}
