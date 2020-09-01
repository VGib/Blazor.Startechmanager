using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Startechmanager.Server.UnitTests
{


    public class BaseTests<T>
    {
        public ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var service = new ServiceCollection();

            foreach (var toInjectProperty in GetMockProperties())
                service.AddTransient(_ =>
                {
                    var mockTypeConstructor = toInjectProperty.PropertyType.GetConstructor(new Type[0]);
                    var createdmock = mockTypeConstructor.Invoke(new object[0]);
                    toInjectProperty.SetValue(this, createdmock);
                    
                    var property = toInjectProperty.PropertyType.GetProperty("Object");
                    return property.GetGetMethod()?.Invoke(this, new object[0]);
                });

            ServiceProvider = service.BuildServiceProvider();

            foreach (var injectedProperty in GetMockProperties())
            {
                injectedProperty.SetValue(this, ServiceProvider.GetRequiredService(injectedProperty.PropertyType));
            }
        }

        private IEnumerable<PropertyInfo> GetMockProperties()
        {
            return this.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Mock<>) && x.CanWrite);
        }

        public T Create()
        {
           return  ServiceProvider.GetRequiredService<T>();
        }

    }

}
