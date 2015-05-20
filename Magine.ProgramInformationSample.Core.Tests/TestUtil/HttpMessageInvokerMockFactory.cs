using System.Net.Http;

using Moq;

namespace Magine.ProgramInformationSample.Core.Tests.TestUtil
{
    public sealed class HttpMessageInvokerMockFactory
    {
        internal Mock<HttpMessageInvoker> NewMessageInvokerMock(Mock<HttpMessageHandler> handlerMock = null)
        {
            return new Mock<HttpMessageInvoker>((handlerMock ?? new Mock<HttpMessageHandler>()).Object);
        }
    }
}