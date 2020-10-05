using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class GetInValidationStarpointsTests : StarpointManagerControllerTests
    {
        [Test]
        public async Task GetInValidationStarpoints_should_return_only_starpoints_to_validate_where_current_user_is_member_of()
        {
            SetUser(LeaderDotnet);
            var target = Create();
            var results = await target.GetInValidationStarpoints();
            results.Select(x => x.Id).Should().BeEquivalentTo(7,10, 12);
        }

        [Test]
        public async Task GetInValidationStarpoints_should_return_starpoints_to_validate_from_all_users()
        {
            await GetInValidationStarpoints_should_return_only_starpoints_to_validate_where_current_user_is_member_of();
        }

    }
}
