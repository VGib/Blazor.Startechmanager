using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class BaseTests<T> where T : class
    {
        public ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var service = new ServiceCollection();

            SetMock();

            foreach (var toInjectProperty in GetMockProperties())
            {
                var toInject = toInjectProperty.GetGetMethod()?.Invoke(this,null);

                if (toInject == null)
                {
                    var mockTypeConstructor = toInjectProperty.PropertyType.GetConstructor(new Type[0]);
                    toInject = mockTypeConstructor.Invoke(new object[0]);
                    toInjectProperty.SetValue(this, toInject);
                }

                var property = toInjectProperty.PropertyType.GetProperties().Where(x => x.Name == "Object").First();
                var objectToInvoke = property.GetGetMethod()?.Invoke(toInject, null);

                service.AddTransient(toInjectProperty.PropertyType.GetGenericArguments()[0], _ => objectToInvoke);
            }
            service.AddTransient<T>();
            ServiceProvider = service.BuildServiceProvider();
        }

        protected virtual void SetMock()
        {
        }

        private IEnumerable<PropertyInfo> GetMockProperties()
        {
            return this.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType &&  x.PropertyType.GetGenericTypeDefinition() == typeof(Mock<>) && x.CanWrite);
        }

        public T Create()
        {
           return  ServiceProvider.GetRequiredService<T>();
        }

    }

}
