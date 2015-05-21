using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Moq;

namespace Magine.ProgramInformationSample.Core.Tests.TestUtil
{
    public sealed class HttpMessageInvokerMockBuilder
    {
        private readonly Mock<HttpMessageInvoker> invokerMock;

        internal HttpMessageInvokerMockBuilder(Mock<HttpMessageHandler> handlerMock = null)
        {
            invokerMock = new Mock<HttpMessageInvoker>(
                (handlerMock ?? new Mock<HttpMessageHandler>()).Object);
        }

        internal HttpMessageInvokerMockBuilder WithLoginSetup(
            HttpResponseMessage response,
            Action<HttpRequestMessage, CancellationToken> callback = null)
        {
            var setup = invokerMock.Setup(
                x => x.SendAsync(
                    It.Is<HttpRequestMessage>(r => r.RequestUri.AbsoluteUri == "https://magine.com/api/login/v1/auth/magine"),
                    It.IsAny<CancellationToken>()));
            if(callback != null) setup.Callback(callback);
            setup.Returns(Task.FromResult(response));
            return this;
        }

        internal HttpMessageInvokerMockBuilder WithAiringsSetup(
            HttpResponseMessage response,
            Action<HttpRequestMessage, CancellationToken> callback = null)
        {
            var setup = invokerMock.Setup(
                x => x.SendAsync(
                    It.Is<HttpRequestMessage>(r => r.RequestUri.AbsoluteUri.StartsWith("https://magine.com/api/content/v2/timeline/airings")),
                    It.IsAny<CancellationToken>()));
            if (callback != null) setup.Callback(callback);
            setup.Returns(Task.FromResult(response));
            return this;
        }


        internal Mock<HttpMessageInvoker> Build()
        {
            return invokerMock;
        }
    }
}