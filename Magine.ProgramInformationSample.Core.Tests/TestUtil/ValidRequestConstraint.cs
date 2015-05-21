using System;
using System.Net.Http;

using NUnit.Framework.Constraints;

namespace Magine.ProgramInformationSample.Core.Tests.TestUtil
{
    internal sealed class ValidRequestConstraint : Constraint
    {
        private readonly string expectedContent;

        private readonly string expectedAuthorizationHeader;

        private readonly HttpMethod expectedMethod;

        private readonly Uri expectedUri;

        public ValidRequestConstraint(
            HttpMethod expectedMethod,
            Uri expectedUri,
            string expectedContent,
            string expectedAuthorizationHeader = null)
        {
            this.expectedMethod = expectedMethod;
            this.expectedUri = expectedUri;
            this.expectedContent = expectedContent;
            this.expectedAuthorizationHeader = expectedAuthorizationHeader;
        }

        public override bool Matches(object actualValue)
        {
            actual = actualValue;
            if (!(actualValue is HttpRequestMessage))
            {
                return false;
            }
            if (ReferenceEquals(actual, null))
            {
                return false;
            }

            var actualRequest = (HttpRequestMessage)actualValue;
            if (expectedMethod != actualRequest.Method)
            {
                return false;
            }
            if (expectedUri.AbsoluteUri != actualRequest.RequestUri.AbsoluteUri)
            {
                return false;
            }
            string actualContent = null;
            if (actualRequest.Content != null)
            {
                actualContent = actualRequest.Content.ReadAsStringAsync().Result;
            }
            if (string.CompareOrdinal(expectedContent, actualContent) != 0)
                return false;
            if (expectedAuthorizationHeader != null)
            {
                string actualHeader = actualRequest.Headers.Authorization.ToString();
                return string.CompareOrdinal(expectedAuthorizationHeader, actualHeader) == 0;
            }
            return true;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteMessageLine("Expected a {0} request on URI: {1}", expectedMethod, expectedUri);
            writer.WriteMessageLine("Expected content:{0}{1}", Environment.NewLine, expectedContent ?? "<null>");
            writer.WriteActualValue(actual);
        }
    }
}