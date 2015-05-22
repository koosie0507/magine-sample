using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Magine.ProgramInformationSample.Core.Model;

using Newtonsoft.Json;

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

        protected override HttpRequestMessage NewRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetAiringsUri());
            request.Headers.Add("Authorization", string.Format("Bearer {0}", authToken));
            return request;
        }

        protected override IEnumerable<Airing> TransformContent(HttpContent content)
        {
            string json = content.ReadAsStringAsync().Result;
            var dict = JsonConvert.DeserializeObject<Dictionary<string, Airing[]>>(json);
            return dict.SelectMany(x => x.Value);
        }
    }
}