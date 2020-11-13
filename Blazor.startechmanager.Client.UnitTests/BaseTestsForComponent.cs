using Bunit;
using Bunit.TestDoubles;
using Common.UnitTests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class BaseTestsForComponent<T> : BaseTests<T> where T : ComponentBase
    {
        public class NavigationManagerForTest : NavigationManager
        {
            public NavigationManagerForTest() : base()
            {
                Initialize("http://localhost/", "http://localhost/");
            }

            public string NavigatedUrl { get; set; } = string.Empty;

            protected override void NavigateToCore(string uri, bool forceLoad)
            {
                NavigatedUrl = uri;
            }
        }

        public MockHttpMessageHandler MockHttp { get; private set; }

        public EventCallbackFactory EventCallbackFactory { get; } = new EventCallbackFactory();

        public NavigationManager NavigationManager { get; private set; }

        public string HasNavigatedToUrl => ((NavigationManagerForTest)NavigationManager)?.NavigatedUrl;

        [SetUp]
        public void SetupHttpClient()
        {
            MockHttp = new MockHttpMessageHandler();
            HttpClient httpClient = MockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            ServiceCollection.AddSingleton(httpClient);

            NavigationManager = new NavigationManagerForTest();
            ServiceCollection.AddSingleton<NavigationManager>(NavigationManager);
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
