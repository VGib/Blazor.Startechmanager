using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Startechmanager.Server.UnitTests.Models
{
    public class IsNotZeroAttributeTests
    {

        [Test]
        public void Non_zero_values_should_be_success()
        {
            var target = new IsNotZeroAttribute();
            var result = target.IsValid(15);
            result.Should().BeTrue();
        }

        [Test]
        public void zero_values_should_be_faillure()
        {
            var target = new IsNotZeroAttribute();
            var result = target.IsValid(0);
            result.Should().BeFalse();
        }
    }
}
