using Blazor.Startechmanager.Server.Data;
using EntityFrameworkCore.Testing.Moq.Helpers;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        public ServiceCollection ServiceCollection { get; private set; }

        public ApplicationDbContext DbContext { get; set; }

        [SetUp]
        public void SetUp()
        {
            ServiceCollection = new ServiceCollection();

            DbContext = new MockedDbContextBuilder<ApplicationDbContext>().UseConstructorWithParameters(
                new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
                new OptionsWrapper<OperationalStoreOptions>(new OperationalStoreOptions()))
                .MockedDbContext;
            ServiceCollection.AddSingleton(_ => DbContext);

            SetMock();

            foreach (var toInjectProperty in GetMockProperties())
            {
                var toInject = toInjectProperty.GetGetMethod()?.Invoke(this, null);

                if (toInject == null)
                {
                    var mockTypeConstructor = toInjectProperty.PropertyType.GetConstructor(new Type[0]);
                    toInject = mockTypeConstructor.Invoke(new object[0]);
                    toInjectProperty.SetValue(this, toInject);
                }

                var property = toInjectProperty.PropertyType.GetProperties().First(x => x.Name == "Object");
                var objectToInvoke = property.GetGetMethod()?.Invoke(toInject, null);

                ServiceCollection.AddTransient(toInjectProperty.PropertyType.GetGenericArguments()[0], _ => objectToInvoke);
            }
            ServiceCollection.AddTransient<T>();
        }

        protected virtual void SetMock()
        {
        }

        private IEnumerable<PropertyInfo> GetMockProperties()
        {
            return this.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Mock<>) && x.CanWrite);
        }

        public T Create()
        {
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
