using Bunit;
using Bunit.Rendering;
using Bunit.TestDoubles.JSInterop;
using Common.UnitTests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net.Http;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class BaseTestsForComponent<T> : BaseTests<T> where T : ComponentBase
    { 
        public MockHttpMessageHandler MockHttp { get; private set; }

        [SetUp]
        public void SetupHttpClient()
        {
            MockHttp = new MockHttpMessageHandler();
            HttpClient httpClient = MockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            ServiceCollection.AddSingleton(httpClient);
        }

        public  IRenderedComponent<T> CreateComponent(params ComponentParameter[] parameters)
        {
            var testContext = new Bunit.TestContext();
            
            foreach(var serviceDescription in ServiceCollection)
            {
                testContext.Services.Add(serviceDescription);
            }
            testContext.Services.AddMockJSRuntime();

            return testContext.RenderComponent<T>(parameters);
        }
    }
}
