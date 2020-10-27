using Blazor.Startechmanager.Client.Services;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Threading.Tasks;
using Blazor.Startechmanager.Client.Helpers;
using System.Net.Http;
using System.Net;
using FluentAssertions;

namespace Blazor.startechmanager.Client.UnitTests
{
    public class HttpClientExtensionTests 
    {
        [Test]
        public async Task when_web_api_return_no_error_on_get_method_no_error_message_should_be_displayed()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Get, "http://localhost/sample").Respond(HttpStatusCode.OK);

            await httpClient.DoActionByGetMethod("sample", messageDisplayer.Object);
            messageDisplayer.Verify(x => x.DisplayErrorMessage(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task when_web_api_return_an_error_on_get_method_an_error_message_should_be_displayed()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Get, "http://localhost/sample").Respond(HttpStatusCode.BadRequest,"text/html","ERROR");

            await httpClient.DoActionByGetMethod("sample", messageDisplayer.Object);
            messageDisplayer.Verify(x => x.DisplayErrorMessage(It.IsIn<string>("ERROR")), Times.Once);
        }

        [Test]
        public async Task when_web_api_return_no_error_on_post_method_no_error_message_should_be_displayed()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Post, "http://localhost/sample").Respond(HttpStatusCode.OK);

            await httpClient.DoActionByPost("sample",new object(), messageDisplayer.Object);
            messageDisplayer.Verify(x => x.DisplayErrorMessage(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task when_web_api_return_an_error_on_post_method_an_error_message_should_be_displayed()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Post, "http://localhost/sample").Respond(HttpStatusCode.BadRequest, "text/html", "ERROR");

            await httpClient.DoActionByPost("sample",new object(), messageDisplayer.Object);
            messageDisplayer.Verify(x => x.DisplayErrorMessage(It.IsIn<string>("ERROR")), Times.Once);
        }

        [Test]
        public async Task when_web_api_return_no_error_on_get_method_should_return_done_status()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Get, "http://localhost/sample").Respond(HttpStatusCode.OK);

            var result = await httpClient.DoActionByGetMethod("sample", messageDisplayer.Object);
            result.Should().Be(ActionStatus.Done);
        }

        [Test]
        public async Task when_web_api_return_an_error_on_get_method_should_return_failled_status()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Get, "http://localhost/sample").Respond(HttpStatusCode.BadRequest, "text/html", "ERROR");

            var result = await httpClient.DoActionByGetMethod("sample", messageDisplayer.Object);
            result.Should().Be(ActionStatus.Failed);
        }

        [Test]
        public async Task when_web_api_return_no_error_on_post_method_should_return_done_status()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Post, "http://localhost/sample").Respond(HttpStatusCode.OK);

            var result = await httpClient.DoActionByPost("sample", new object(), messageDisplayer.Object);
            result.Should().Be(ActionStatus.Done);
        }

        [Test]
        public async Task when_web_api_return_an_error_on_post_method_should_return_failled_status()
        {
            var messageDisplayer = new Mock<IMessageDisplayer>();
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            mockHttp.Expect(HttpMethod.Post, "http://localhost/sample").Respond(HttpStatusCode.BadRequest, "text/html", "ERROR");

            var result = await httpClient.DoActionByPost("sample", new object(), messageDisplayer.Object);
            result.Should().Be(ActionStatus.Failed);
        }
    }
}
