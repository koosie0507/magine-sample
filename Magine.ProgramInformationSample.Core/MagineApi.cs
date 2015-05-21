using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Magine.ProgramInformationSample.Core.Handlers;
using Magine.ProgramInformationSample.Core.Model;

namespace Magine.ProgramInformationSample.Core
{
    public sealed class MagineApi : IMagineApi
    {
        private readonly HttpMessageInvoker httpMessageInvoker;

        private string authToken;

        public MagineApi() : this(NewClient()) { }

        public MagineApi(HttpMessageInvoker httpMessageInvoker)
        {
            if(httpMessageInvoker == null) throw new ArgumentNullException();
            this.httpMessageInvoker = httpMessageInvoker;
        }

        private static HttpClient NewClient()
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

        public async Task Login(string userName, string password)
        {
            authToken = await new LoginHandler(httpMessageInvoker, userName, password).InvokeAsync();
        }

        public async Task<IEnumerable<Airing>> GetAirings(DateTime from, DateTime to)
        {
            return await new AiringsHandler(httpMessageInvoker, authToken, from, to).InvokeAsync();
        }
    }
}