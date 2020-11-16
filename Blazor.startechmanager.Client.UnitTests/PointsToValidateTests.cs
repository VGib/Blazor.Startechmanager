using Blazor.Startechmanager.Client.Pages;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class PointsToValidateTests : BaseTestsForComponent<PointsToValidate>
    {
        public Mock<IStartechAuthorizationService> StartechAuthorizationService { get; set; }

        public Mock<IMessageDisplayer> MessageDisplayer { get; set; }

        public Mock<IConfirmDisplayer> ConfirmDisplayer { get; set; }

        [Test]
        public async Task the_points_to_validate_should_be_load()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetInValidationStarpoints")
                    .RespondValues(new[] { new StarpointsItem() });
            var target = CreateComponent();
            await Task.Delay(50);
            target.Instance.Items.Should().HaveCount(1);
        }

        [Test]
        public async Task when_the_points_to_validate_is_load_isLoad_should_be_true()
        {
            MockHttp.Expect(HttpMethod.Get, "http://localhost/StarpointsManager/GetInValidationStarpoints")
                  .RespondValues(new[] { new StarpointsItem() });
            var target = CreateComponent();
            await Task.Delay(50);
            target.Instance.IsLoad.Should().BeTrue();
        }
    }
}
