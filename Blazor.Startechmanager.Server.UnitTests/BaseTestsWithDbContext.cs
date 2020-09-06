using Blazor.Startechmanager.Server.Data;
using Common.UnitTests;
using EntityFrameworkCore.Testing.Moq.Helpers;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class BaseTestsWithDbContext<T> : BaseTests<T> where T : class
    {
        public ApplicationDbContext DbContext { get; set; }

       [SetUp]
        public void SetUpDbContext()
        { 
            DbContext = new MockedDbContextBuilder<ApplicationDbContext>().UseConstructorWithParameters(
                new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
                new OptionsWrapper<OperationalStoreOptions>(new OperationalStoreOptions()))
                .MockedDbContext;
            ServiceCollection.AddSingleton(_ => DbContext);
        }
    }
}
