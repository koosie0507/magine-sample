using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Magine.ProgramInformationSample.Core.Handlers
{
    public sealed class LoginHandler : Handler<string>
    {
        private const string LoginUrl = "https://magine.com/api/login/v1/auth/magine";

        private readonly string password;

        private readonly string userName;

        public LoginHandler(HttpMessageInvoker invoker, string userName, string password)
            : base(invoker)
        {
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new ArgumentOutOfRangeException();
            this.userName = userName;
            this.password = password;
        }

        protected override HttpRequestMessage NewRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUrl);
            string json = "{\"identity\":\"" + userName + "\", \"accessKey\":\"" + password + "\"}";
            request.Content = new StringContent(json);
            return request;
        }

        protected override string TransformContent(HttpContent content)
        {
            var deserializer = new DataContractJsonSerializer(typeof(LoginResponse));
            using (Stream s = content.ReadAsStreamAsync().Result)
                return ((LoginResponse)deserializer.ReadObject(s)).SessionId;
        }

        [DataContract]
        internal class LoginResponse
        {
            [DataMember(Name = "sessionId")]
            internal string SessionId;
        }
    }
}