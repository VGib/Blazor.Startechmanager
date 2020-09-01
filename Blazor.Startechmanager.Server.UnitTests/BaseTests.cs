using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class InjectForTestAttribute : Attribute
    {
    }

    public class BaseTests
    {
        public ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var service = new ServiceCollection();

            foreach (var toInjectProperty in this.GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(InjectForTestAttribute), true) != null))
            {
                service.AddTransient(_ => Substitute.For(new Type[] { toInjectProperty.PropertyType }, new object[0]));
            }

            ServiceProvider = service.BuildServiceProvider();

            foreach (var injectedProperty in this.GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(InjectForTestAttribute), true) != null && x.CanWrite))
            {
                injectedProperty.SetValue(this, ServiceProvider.GetRequiredService(injectedProperty.PropertyType));
            }
        }
    }
}
