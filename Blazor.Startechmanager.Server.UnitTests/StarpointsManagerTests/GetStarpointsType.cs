using Blazor.Startechmanager.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class StarpointManagerControllerTests_GetStarpointsType : StarpointManagerControllerTests
    {
        [Test]
        public async Task GetStarpointsTypes_should_return_all_active_starpointTypes_of_the_startech()
        {
            var target = Create();
            var results = await target.GetStarpointsType();
            results.Select(x => x.TypeName).Should().BeEquivalentTo("Blog Article", "Course", "Presentation");
        }

        [Test]
        public async Task GetStarpointsTypes_can_be_called_by_every_body()
        {
            var thisMethod = typeof(StarPointsManagerController).GetMethod(nameof(StarPointsManagerController.GetStarpointsType));
            var attributes = thisMethod.GetCustomAttributes(false);
            attributes.Where(x => x.GetType().IsAssignableFrom(typeof(IAuthorizationFilter)) || x.GetType().IsAssignableFrom(typeof(IAsyncAuthorizationFilter))).Should().BeEmpty();
        }

    }
}
