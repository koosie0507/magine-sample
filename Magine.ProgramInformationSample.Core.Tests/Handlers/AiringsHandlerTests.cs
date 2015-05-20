using System;

using Magine.ProgramInformationSample.Core.Handlers;
using Magine.ProgramInformationSample.Core.Tests.TestUtil;

using NUnit.Framework;

namespace Magine.ProgramInformationSample.Core.Tests.Handlers
{
    [TestFixture]
    public sealed class AiringsHandlerTests
    {
        private static readonly DateTime FromDate = DateTime.Now.Date;

        private static readonly DateTime ToDate = DateTime.Now.AddDays(1).Date;

        private static readonly HttpMessageInvokerMockFactory InvokerMockFactory = new HttpMessageInvokerMockFactory();

        [Test]
        public void Ctor_NullAuthToken_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AiringsHandler(InvokerMockFactory.NewMessageInvokerMock().Object, null, FromDate, ToDate));
        }

        [Test]
        public void GetAirings_NullClient_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AiringsHandler(null, "a", FromDate, ToDate));
        }
    }
}