using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core
{
    public sealed class LoginHandler
    {
        private const string LoginUrl = "https://magine.com/api/login/v1/auth/magine";

        internal Task<HttpResponseMessage> PerformLoginRequest(HttpClient httpClient)
        {
            string json = "{\"identity\":\"maginemobdevtest@magine.com\", \"accessKey\":\"magine\"}";
            return httpClient.PostAsync(LoginUrl, new StringContent(json));
        }

        [DataContract]
        internal class LoginResponse
        {
            [DataMember(Name = "sessionId")]
            internal string SessionId;
        }

        public string GetAuthToken(HttpResponseMessage responseMessage)
        {
            var deserializer = new DataContractJsonSerializer(typeof(LoginHandler.LoginResponse));
            using (Stream s = responseMessage.Content.ReadAsStreamAsync().Result) return ((LoginHandler.LoginResponse)deserializer.ReadObject(s)).SessionId;
        }
    }
}