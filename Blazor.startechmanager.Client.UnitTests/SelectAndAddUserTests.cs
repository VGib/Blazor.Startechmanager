using Blazor.Startechmanager.Client.Component;
using Blazor.Startechmanager.Shared.Models;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class SelectAndAddUserTests : BaseTestsForComponent<SelectAndAddUser>
    {
        [Test]
        public async Task when_no_user_is_select_adding_a_user_shoud_do_nothing()
        {
            var hasEventCallBackBeenCalled = false;
            var eventCallback = ComponentParameterFactory.EventCallback<UserObject>("Add", _ => hasEventCallBackBeenCalled = true);
            var target = CreateComponent(eventCallback);
            target.Instance.UserObjectToAdd = null;
            await target.Instance.AddUser();
            hasEventCallBackBeenCalled.Should().BeFalse();
        }

        [Test]
        public async Task when_no_user_is_select_adding_a_user_shoud_call_the_add_eventcallback_with_the_user()
        {
            var hasEventCallBackBeenCalled = false;
            var eventCallback = ComponentParameterFactory.EventCallback<UserObject>("Add", _ => hasEventCallBackBeenCalled = true);
            var target = CreateComponent(eventCallback);
            target.Instance.UserObjectToAdd = new UserObject { Id = 171, UserName = "OTTO" };
            await target.Instance.AddUser();
            hasEventCallBackBeenCalled.Should().BeTrue();
        }
    }
}
