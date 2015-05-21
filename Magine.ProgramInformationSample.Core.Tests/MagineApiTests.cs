using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

using Magine.ProgramInformationSample.Core.Model;
using Magine.ProgramInformationSample.Core.Tests.Properties;
using Magine.ProgramInformationSample.Core.Tests.TestUtil;

using Moq;

using NUnit.Framework;

namespace Magine.ProgramInformationSample.Core.Tests
{
    [TestFixture]
    public class MagineApiTests
    {
        private readonly HttpMessageInvokerMockBuilder invokerMockBuilder = new HttpMessageInvokerMockBuilder();
        private readonly DateTime fromDate = new DateTime(2014, 5, 5, 0, 0, 0);
        private readonly DateTime toDate = new DateTime(2014, 5, 6, 0, 0, 0);

        private static HttpResponseMessage NewSuccessResponse(string content)
        {
            var responseContent = new StringContent(content);
            var sessionIdResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };
            return sessionIdResponse;
        }

        private static HttpResponseMessage NewSessionIdResponse(string sessionId)
        {
            return NewSuccessResponse("{\"sessionId\":\"" + sessionId + "\"}");
        }

        [Test]
        public void Ctor_WithNullInvoker_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MagineApi(null));
        }

        [Test]
        public void GetAirings_WhenAuthenticated_ReturnsAirings()
        {
            var sut = new MagineApi();
            sut.Login("maginemobdevtest@magine.com", "magine").Wait();

            List<Airing> airings = sut.GetAirings(DateTime.Now, DateTime.Now.AddDays(1)).Result.ToList();

            Assert.That(airings, Has.Count.GreaterThan(0));
        }

        [Test]
        public void Login_HappyFlow_IsSuccessful()
        {
            HttpRequestMessage expectedRequest = null;
            Mock<HttpMessageInvoker> invokerMock = new HttpMessageInvokerMockBuilder()
                .WithLoginSetup(NewSessionIdResponse("a"), (r, c) => expectedRequest = r)
                .Build();
            var sut = new MagineApi(invokerMock.Object);

            Assert.DoesNotThrow(sut.Login("a", "b").Wait);

            Assert.That(
                expectedRequest,
                new ValidRequestConstraint(
                    HttpMethod.Post,
                    new Uri("https://magine.com/api/login/v1/auth/magine"),
                    "{\"identity\":\"a\", \"accessKey\":\"b\"}"));
        }

        [Test]
        public void Login_UnsuccessfulResponse_ThrowsHttpException()
        {
            StringContent responseContent = new StringContent("{\"sessionId\":\"a\"");
            var badRequestMessage = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = responseContent };
            Mock<HttpMessageInvoker> invokerMock = invokerMockBuilder
                .WithLoginSetup(badRequestMessage)
                .Build();
            var sut = new MagineApi(invokerMock.Object);

            var e = Assert.Throws<AggregateException>(sut.Login("a", "b").Wait);

            Assert.That(e.InnerExceptions.First(), Is.InstanceOf<HttpRequestException>());
        }

        [Test]
        public void Login_NullResponse_ThrowsHttpException()
        {
            Mock<HttpMessageInvoker> invokerMock = invokerMockBuilder
                .WithLoginSetup(new HttpResponseMessage(HttpStatusCode.OK))
                .Build();
            var sut = new MagineApi(invokerMock.Object);

            var e = Assert.Throws<AggregateException>(sut.Login("a", "b").Wait);

            Assert.That(e.InnerExceptions.First(), Is.InstanceOf<HttpRequestException>());
        }

        [Test]
        public void GetAirings_HappyFlow_UsesAppropriateRequest()
        {
            const string ExpectedSessionId = "a";
            HttpRequestMessage expectedRequest = null;
            HttpResponseMessage sessionIdResponse = NewSessionIdResponse(ExpectedSessionId);
            Mock<HttpMessageInvoker> invokerMock = invokerMockBuilder
                .WithLoginSetup(sessionIdResponse)
                .WithAiringsSetup(NewSuccessResponse("{\"1\":[]}"), (r,c)=>expectedRequest = r)
                .Build();
            var sut = new MagineApi(invokerMock.Object);
            sut.Login("a", "b").Wait();

            sut.GetAirings(fromDate, toDate).Wait();

            Assert.That(expectedRequest, new ValidRequestConstraint(
                HttpMethod.Get,
                new Uri("https://magine.com/api/content/v2/timeline/airings?from=20140504T210000Z&to=20140505T210000Z"),
                null,
                "Bearer a"));
        }

        [Test]
        public void GetAirings_NoAiringsReturned_ApiReturnsEmptySeries()
        {
            HttpResponseMessage sessionIdResponse = NewSessionIdResponse("b");
            Mock<HttpMessageInvoker> invokerMock = invokerMockBuilder
                .WithLoginSetup(sessionIdResponse)
                .WithAiringsSetup(NewSuccessResponse("{\"1\":[]}"))
                .Build();
            var sut = new MagineApi(invokerMock.Object);
            sut.Login("a", "b").Wait();

            IEnumerable<Airing> actual = sut.GetAirings(fromDate, toDate).Result;

            Assert.That(actual, Is.Empty);
        }

        private static string GetResourceContent(byte[] contentBytes)
        {
            using(var ms = new MemoryStream(contentBytes))
            using (var sr = new StreamReader(ms))
                return sr.ReadToEnd();
        }

        private IEnumerable<TestCaseData> GetAiringsTestCases()
        {
            yield return new TestCaseData(GetResourceContent(Resources.OneAiring), 1);
            yield return new TestCaseData(GetResourceContent(Resources.TwoAiringsOnDifferentChannels), 2);
            yield return new TestCaseData(GetResourceContent(Resources.TwoAiringsOnSameChannel), 2);
        }

        [Test]
        [TestCaseSource("GetAiringsTestCases")]
        public void GetAirings_OneAiringReturned_ApiReturnsSeriesWithOneElement(
            string responseContent, int expectedAiringCount)
        {
            HttpResponseMessage sessionIdResponse = NewSessionIdResponse("a");
            Mock<HttpMessageInvoker> invokerMock = invokerMockBuilder
                .WithLoginSetup(sessionIdResponse)
                .WithAiringsSetup(NewSuccessResponse(responseContent))
                .Build();
            var sut = new MagineApi(invokerMock.Object);
            sut.Login("a", "b").Wait();

            IEnumerable<Airing> actual = sut.GetAirings(fromDate, toDate).Result;

            Assert.That(actual, Has.Exactly(expectedAiringCount).InstanceOf<Airing>());
        }
    }
}