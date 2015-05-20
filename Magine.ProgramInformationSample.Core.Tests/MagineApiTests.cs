using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

using Magine.ProgramInformationSample.Core.Model;

using NUnit.Framework;

namespace Magine.ProgramInformationSample.Core.Tests
{
    [TestFixture]
    public class MagineApiTests
    {
         [Test]
        public void Login_WithCorrectParams_IsSuccessful()
        {
            var sut = new MagineApi();

            Assert.DoesNotThrow(sut.Login("maginemobdevtest@magine.com", "magine").Wait);
        }

        [Test]
        public void GetAirings_WhenAuthenticated_ReturnsAirings()
        {
            var sut = new MagineApi();
            sut.Login("maginemobdevtest@magine.com", "magine").Wait();

            List<Airing> airings = sut.GetAirings(DateTime.Now, DateTime.Now.AddDays(1)).Result.ToList();

            Assert.That(airings, Has.Count.GreaterThan(0));
        }
    }
}