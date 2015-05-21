using System;
using System.Collections;
using System.Linq;
using System.Net;

using Magine.ProgramInformationSample.Core.ViewModel;

using Moq;

using NUnit.Framework;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Magine.ProgramInformationSample.Core.Tests.Features.Steps
{
    [Binding]
    public class LoginSteps
    {
        private const string CredentialsKey = "c";

        private const string MagineApiKey = "api";

        private const string RouterKey = "router";

        private LoginViewModel viewModel;

        private Mock<IMagineApi> NewExceptionThrowingMock()
        {
            Mock<IMagineApi> mock = new Mock<IMagineApi>();
            mock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            return mock;
        }

        private void SetupMocks(Mock<IMagineApi> apiMock = null, Mock<IRouter> routerMock = null)
        {
            ScenarioContext.Current.Add(MagineApiKey, apiMock ?? new Mock<IMagineApi>());
            ScenarioContext.Current.Add(RouterKey, routerMock ?? new Mock<IRouter>());
        }

        private T GetMockObject<T>(string key) where T : class
        {
            Mock<T> mock = ScenarioContext.Current.Get<Mock<T>>(key);
            return mock.Object;
        }

        [Given(@"I have entered the following login data:")]
        public void GivenIHaveEnteredTheFollowingLoginData(Table table)
        {
            var credentials = table.CreateInstance<NetworkCredential>();
            ScenarioContext.Current.Add(CredentialsKey, credentials);
            SetupMocks();
        }

        [Given(@"I have not entered user name")]
        public void GivenIHaveNotEnteredUserName()
        {
            ScenarioContext.Current.Add(CredentialsKey, new NetworkCredential("", "some pass"));
            SetupMocks(NewExceptionThrowingMock());
        }

        [Given(@"I have not entered password")]
        public void GivenIHaveNotEnteredPassword()
        {
            ScenarioContext.Current.Add(CredentialsKey, new NetworkCredential("some user", ""));
            SetupMocks(NewExceptionThrowingMock());
        }

        [Given(@"I have entered invalid user name or password")]
        public void GivenIHaveEnteredInvalidUserNameOrPassword()
        {
            ScenarioContext.Current.Add(CredentialsKey, new NetworkCredential("a", "b"));
            SetupMocks(NewExceptionThrowingMock());
        }

        [When(@"I log in")]
        public void WhenILogIn()
        {
            NetworkCredential credentials = ScenarioContext.Current.Get<NetworkCredential>(CredentialsKey);
            viewModel = new LoginViewModel(GetMockObject<IMagineApi>(MagineApiKey), GetMockObject<IRouter>(RouterKey));
            viewModel.Login(credentials);
        }

        [Then(@"Magine API is called with expected user name and password")]
        public void ThenMagineApiIsCalled()
        {
            Mock<IMagineApi> magineApiMock = ScenarioContext.Current.Get<Mock<IMagineApi>>(MagineApiKey);
            var credential = ScenarioContext.Current.Get<NetworkCredential>(CredentialsKey);
            magineApiMock.Verify(x => x.Login(credential.UserName, credential.Password), Times.Once);
        }

        [Then(@"I will be logged in")]
        public void ThenIWillBeLoggedIn()
        {
            Assert.IsFalse(viewModel.ErrorInfo.HasErrors);
            Assert.That(viewModel.ErrorInfo.GetErrors("LoginError"), Is.Empty);
            Mock<IRouter> routerMock = ScenarioContext.Current.Get<Mock<IRouter>>(RouterKey);
            routerMock.Verify(x => x.GoTo<ProgramInformationViewModel>(), Times.Once);
        }

        [Then(@"I will not be logged in")]
        public void ThenIWillNotBeLoggedIn()
        {
            Assert.IsTrue(viewModel.ErrorInfo.HasErrors);
            Mock<IRouter> routerMock = ScenarioContext.Current.Get<Mock<IRouter>>(RouterKey);
            routerMock.Verify(x => x.GoTo<ProgramInformationViewModel>(), Times.Never);
        }

        [Then(@"I receive an error stating ""(.*)""")]
        public void ThenIReceiveAnErrorStatingINeedToEnterCredentials(string enterCredentialsError)
        {
            IEnumerable errors = viewModel.ErrorInfo.GetErrors("LoginError");
            Assert.AreEqual(enterCredentialsError, errors.OfType<string>().First());
        }
    }
}