using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core
{
    public sealed class AiringsHandler
    {
        private const string AiringsUrl = "https://magine.com/api/content/v2/timeline/airings";

        private MagineApi magineApi;

        public AiringsHandler(MagineApi magineApi)
        {
            this.magineApi = magineApi;
        }

        private Uri GetAiringsUri(DateTime from, DateTime to)
        {
            var uriBuilder = new UriBuilder(AiringsUrl);
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("from={0:yyyyMMddTHHmmssZ}", @from.Date.ToUniversalTime());
            queryBuilder.AppendFormat("&to={0:yyyyMMddTHHmmssZ}", to.Date.ToUniversalTime());
            uriBuilder.Query = queryBuilder.ToString();
            return uriBuilder.Uri;
        }

        private Task<HttpResponseMessage> PerformGetAirings(HttpMessageInvoker client, DateTime from, DateTime to)
        {
            return client.SendAsync(magineApi.GetAiringsRequestMessage(GetAiringsUri(@from, to)), new CancellationToken());
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
    }
}