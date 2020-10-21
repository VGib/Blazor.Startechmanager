using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Blazor.Startechmanager.Server.UnitTests.Models
{
    public class RequiredEnumAttributeTests
    {
        [Test]
        public void when_value_is_an_enum_and_a_valid_enum_value_result_should_be_success()
        {
            var target = new RequiredEnumAttribute(typeof(ValidationState));
            var result = target.IsValid(ValidationState.Refused);
            result.Should().BeTrue();
        }
        [Test]
        public void when_value_is_an_enum_and_not_a_valid_enum_value_result_should_be_faillure()
        {
            var target = new RequiredEnumAttribute(typeof(ValidationState));
            var result = target.IsValid((ValidationState) int.MaxValue);
            result.Should().BeFalse();
        }
    }
}
