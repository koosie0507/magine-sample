using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core
{
    public sealed class MagineApi : IMagineApi
    {
        private const string LoginUrl = "https://magine.com/api/login/v1/auth/magine";

        private const string AiringsUrl = "https://magine.com/api/content/v2/timeline/airings";

        private string authToken;

        public async Task Login(string userName, string password)
        {
            using (HttpResponseMessage message = await PerformLoginRequest(NewClient()))
            {
                authToken = GetAuthToken(message);
            }
        }

        private Task<HttpResponseMessage> PerformLoginRequest(HttpClient httpClient)
        {
            string json = "{\"identity\":\"maginemobdevtest@magine.com\", \"accessKey\":\"magine\"}";
            return httpClient.PostAsync(LoginUrl, new StringContent(json));
        }

        private HttpClient NewClient()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler, true);
            client.DefaultRequestHeaders.Add("User-Agent", "X-Magine-ProgramInfoSample");
            return client;
        }

        private string GetAuthToken(HttpResponseMessage responseMessage)
        {
            var deserializer = new DataContractJsonSerializer(typeof(LoginResponse));
            using (Stream s = responseMessage.Content.ReadAsStreamAsync().Result) return ((LoginResponse)deserializer.ReadObject(s)).SessionId;
        }

        private HttpRequestMessage GetAiringsRequestMessage(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", authToken));
            return request;
        }

        private Uri GetAiringsUri(DateTime from, DateTime to)
        {
            var uriBuilder = new UriBuilder(AiringsUrl);
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("from={0:yyyyMMddTHHmmssZ}", from.Date.ToUniversalTime());
            queryBuilder.AppendFormat("&to={0:yyyyMMddTHHmmssZ}", to.Date.ToUniversalTime());
            uriBuilder.Query = queryBuilder.ToString();
            return uriBuilder.Uri;
        }

        private Task<HttpResponseMessage> PerformGetAirings(HttpMessageInvoker client, DateTime from, DateTime to)
        {
            return client.SendAsync(GetAiringsRequestMessage(GetAiringsUri(from, to)), new CancellationToken());
        }

        public async Task<IEnumerable<Airing>> GetAirings(DateTime from, DateTime to)
        {
            DataContractJsonSerializer serializer = NewAiringsSerializer();
            using (HttpResponseMessage message = await PerformGetAirings(NewClient(), from, to))
            {
                using (Stream jsonStream = await message.Content.ReadAsStreamAsync())
                {
                    Dictionary<string, Airing[]> airingsPerChannel =
                        (Dictionary<string, Airing[]>)serializer.ReadObject(jsonStream);
                    return airingsPerChannel.Values.SelectMany(x => x);
                }
            }
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

        [DataContract]
        internal class LoginResponse
        {
            [DataMember(Name = "sessionId")]
            internal string SessionId;
        }
    }
}