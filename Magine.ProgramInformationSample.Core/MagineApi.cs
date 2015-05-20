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
        private string authToken;

        private readonly LoginHandler loginHandler = new LoginHandler();

        private readonly AiringsHandler airingsHandler;

        public MagineApi()
        {
            airingsHandler = new AiringsHandler(this);
        }

        public async Task Login(string userName, string password)
        {
            using (HttpResponseMessage message = await loginHandler.PerformLoginRequest(NewClient()))
            {
                authToken = loginHandler.GetAuthToken(message);
            }
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

        internal HttpRequestMessage GetAiringsRequestMessage(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", authToken));
            return request;
        }

        public async Task<IEnumerable<Airing>> GetAirings(DateTime from, DateTime to)
        {
            DataContractJsonSerializer serializer = AiringsHandler.NewAiringsSerializer();
            using (HttpResponseMessage message = await airingsHandler.PerformGetAirings(NewClient(), from, to))
            {
                using (Stream jsonStream = await message.Content.ReadAsStreamAsync())
                {
                    Dictionary<string, Airing[]> airingsPerChannel =
                        (Dictionary<string, Airing[]>)serializer.ReadObject(jsonStream);
                    return airingsPerChannel.Values.SelectMany(x => x);
                }
            }
        }
    }
}