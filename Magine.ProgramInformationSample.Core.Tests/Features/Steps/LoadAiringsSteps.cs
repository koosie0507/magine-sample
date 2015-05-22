using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Magine.ProgramInformationSample.Core.Model;
using Magine.ProgramInformationSample.Core.Tests.Properties;
using Magine.ProgramInformationSample.Core.Tests.TestUtil;
using Magine.ProgramInformationSample.Core.ViewModel;

using Moq;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Magine.ProgramInformationSample.Core.Tests.Features.Steps
{
    [Binding]
    public class LoadAiringsSteps
    {
        private ProgramInformationViewModel viewModel;

        private readonly Mock<IMagineApi> magineApiMock = new Mock<IMagineApi>();

        private readonly Mock<IRouter> routerMock = new Mock<IRouter>();

        [Given(@"I am logged in")]
        public void GivenIAmLoggedIn()
        {
            magineApiMock.Setup(x => x.GetAirings(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((IEnumerable<Airing>)new List<Airing>{new Airing()}));
            viewModel = new ProgramInformationViewModel(magineApiMock.Object, Mock.Of<IRouter>());
        }

        [Given(@"I am a non-authenticated user")]
        public void GivenIAmANonAuthenticatedUser()
        {
            magineApiMock.Setup(x => x.GetAirings(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Throws<HttpRequestException>();
            viewModel = new ProgramInformationViewModel(magineApiMock.Object, routerMock.Object);
        }

        [When(@"I request airings")]
        public void WhenIRequestAirings()
        {
            viewModel.LoadAiringsAsync().Wait();
        }

        [Then(@"I should receive a series of airings")]
        public void ThenIShouldReceiveASeriesOfAirings()
        {
            magineApiMock.Verify(x=>x.GetAirings(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            Assert.That(viewModel.Airings, Is.Not.Empty);
            Assert.That(viewModel.Airings, Has.Exactly(1).InstanceOf<AiringViewModel>());
        }

        [Then(@"I am redirected to the login page")]
        public void ThenIAmRedirectedToTheLoginPage()
        {
            Assert.That(viewModel.Airings, Is.Empty);
            routerMock.Verify(x => x.GoTo<LoginViewModel>(), Times.Once);
        }
    }
}