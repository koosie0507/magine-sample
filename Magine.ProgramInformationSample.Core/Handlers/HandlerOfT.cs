using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core.Handlers
{
    public abstract class Handler<T>
    {
        private readonly CancellationToken cancellationToken;

        private readonly HttpMessageInvoker invoker;

        protected Handler(HttpMessageInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException("invoker");

            this.invoker = invoker;
            cancellationToken = new CancellationToken();
        }

        protected abstract HttpRequestMessage NewRequest();

        protected abstract T TransformContent(HttpContent content);

        public async Task<T> InvokeAsync()
        {
            HttpResponseMessage response = await invoker.SendAsync(NewRequest(), cancellationToken);
            response.EnsureSuccessStatusCode();
            if (response.Content == null)
                throw new HttpRequestException();
            return TransformContent(response.Content);
        }
    }
}