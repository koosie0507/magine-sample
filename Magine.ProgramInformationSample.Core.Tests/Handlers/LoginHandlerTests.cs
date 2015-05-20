using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Magine.ProgramInformationSample.Core.Handlers;
using Magine.ProgramInformationSample.Core.Tests.TestUtil;

using Moq;

using NUnit.Framework;

namespace Magine.ProgramInformationSample.Core.Tests.Handlers
{
    [TestFixture]
    public sealed class LoginHandlerTests
    {
        private readonly HttpMessageInvokerMockFactory invokerMockFactory = new HttpMessageInvokerMockFactory();

        [TestCase("")]
        [TestCase(null)]
        public void Ctor_NullOrEmptyUserName_ThrowsArgumentOutOfRangeException(string user)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LoginHandler(invokerMockFactory.NewMessageInvokerMock().Object, user, "b"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Ctor_NullOrEmptyPassword_ThrowsArgumentOutOfRangeException(string pass)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LoginHandler(invokerMockFactory.NewMessageInvokerMock().Object, "a", pass));
        }

        private static bool IsValidLoginRequest(HttpRequestMessage actualRequestMessage, string expectedContent)
        {
            if (!actualRequestMessage.RequestUri.AbsoluteUri.Equals("https://magine.com/api/login/v1/auth/magine"))
            {
                return false;
            }
            if (!expectedContent.Equals(actualRequestMessage.Content.ReadAsStringAsync().Result))
            {
                return false;
            }
            return actualRequestMessage.Method == HttpMethod.Post;
        }

        [Test]
        public void Ctor_NullClient_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LoginHandler(null, "a", "b"));
        }

        [Test]
        public void InvokeAsync_HappyFlow_CallsSendAsyncWithExpectedParams()
        {
            // arrange
            Mock<HttpMessageInvoker> invokerMock = invokerMockFactory.NewMessageInvokerMock();
            var successfulLoginMessage = new HttpResponseMessage(HttpStatusCode.OK)
                                             {
                                                 Content = new StringContent("{\"sessionId\":\"a\"")
                                             };
            Task<HttpResponseMessage> succesCodeTask = Task.FromResult(successfulLoginMessage);
            invokerMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(succesCodeTask);
            var sut = new LoginHandler(invokerMock.Object, "a", "b");

            // act
            sut.InvokeAsync().Wait();

            // verify
            invokerMock.Verify(
                x =>
                x.SendAsync(
                    It.Is<HttpRequestMessage>(
                        request => IsValidLoginRequest(request, "{\"identity\":\"a\", \"accessKey\":\"b\"}")),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public void InvokeAsync_UnsuccessfulResponse_ThrowsHttpRequestException()
        {
            Mock<HttpMessageInvoker> invokerMock = invokerMockFactory.NewMessageInvokerMock();
            var unsuccessfulLoginMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"sessionId\":\"a\"")
            };
            Task<HttpResponseMessage> errorTask = Task.FromResult(unsuccessfulLoginMessage);
            invokerMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(errorTask);
            var sut = new LoginHandler(invokerMock.Object, "a", "b");

            var e = Assert.Throws<AggregateException>(sut.InvokeAsync().Wait);

            Assert.That(e.InnerExceptions.First(), Is.InstanceOf<HttpRequestException>());
        }

        [Test]
        public void InvokeAsync_NullResponseContent_ThrowsHttpRequestException()
        {
            Mock<HttpMessageInvoker> invokerMock = invokerMockFactory.NewMessageInvokerMock();
            var unsuccessfulLoginMessage = new HttpResponseMessage(HttpStatusCode.OK);
            Task<HttpResponseMessage> errorTask = Task.FromResult(unsuccessfulLoginMessage);
            invokerMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(errorTask);
            var sut = new LoginHandler(invokerMock.Object, "a", "b");

            var e = Assert.Throws<AggregateException>(sut.InvokeAsync().Wait);

            Assert.That(e.InnerExceptions.First(), Is.InstanceOf<HttpRequestException>());
        }
    }
}