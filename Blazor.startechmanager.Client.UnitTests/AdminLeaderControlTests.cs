using Blazor.Startechmanager.Client.Component;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using Bunit.Rendering;
using Castle.DynamicProxy.Contributors;
using Common.UnitTests;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class AdminLeaderControlTests : BaseTestsForComponent<AdminLeaderControl>
    { 

        public ComponentParameter StartechType { get; } = ComponentParameterFactory.Parameter(nameof(AdminLeaderControl.StartechType), "Admin");

        public ComponentParameter DisplayName { get; } = ComponentParameterFactory.Parameter(nameof(AdminLeaderControl.DisplayName), "Administrator");


        [Test]
        public void when_loading_leaders_the_razor_component_should_be_refreshed_with_leader()
        {
            throw new NotImplementedException("to do");
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
        public void when_adding_a_leader_the_right_startechtype_api_should_be_called()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public void when_removing_a_leader_the_right_startechtype_api_should_be_called()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public void when_adding_a_leader_the_leader_list_should_be_reload()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public void when_removing_a_leader_the_leader_list_should_be_called()
        {
            throw new NotImplementedException("to do");
        }
    }
}
