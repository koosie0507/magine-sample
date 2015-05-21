using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

using Magine.ProgramInformationSample.Core.Model;

namespace Magine.ProgramInformationSample.Core.Handlers
{
    public sealed class AiringsHandler : Handler<IEnumerable<Airing>>
    {
        private const string AiringsUrlFormat = "https://magine.com/api/content/v2/timeline/airings?from={0:yyyyMMddTHHmmssZ}&to={1:yyyyMMddTHHmmssZ}";

        private readonly string authToken;

        private readonly DateTime @from;

        private readonly DateTime to;

        public AiringsHandler(HttpMessageInvoker invoker, string authToken, DateTime from, DateTime to)
            : base(invoker)
        {
            if (authToken == null) throw new ArgumentNullException();
            this.authToken = authToken;
            this.@from = @from;
            this.to = to;
        }

        private Uri GetAiringsUri()
        {
            return new Uri(String.Format(AiringsUrlFormat, @from.Date.ToUniversalTime(), to.Date.ToUniversalTime()));
        }

        private static DataContractJsonSerializer NewAiringsSerializer()
        {
            return new DataContractJsonSerializer(
                typeof(Dictionary<string, Airing[]>),
                new DataContractJsonSerializerSettings
                    {
                        UseSimpleDictionaryFormat = true,
                        KnownTypes = new[] { typeof(Airing) }
                    });
        }

        protected override HttpRequestMessage NewRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetAiringsUri());
            request.Headers.Add("Authorization", string.Format("Bearer {0}", authToken));
            return request;
        }

        protected override IEnumerable<Airing> TransformContent(HttpContent content)
        {
            DataContractJsonSerializer serializer = NewAiringsSerializer();
            using (Stream jsonStream = content.ReadAsStreamAsync().Result)
            {
                var airingsPerChannel = (Dictionary<string, Airing[]>)serializer.ReadObject(jsonStream);
                return airingsPerChannel.Values.SelectMany(x => x);
            }
        }
    }
}