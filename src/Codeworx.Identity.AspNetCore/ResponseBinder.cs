using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public abstract class ResponseBinder<TResponse> : IResponseBinder<TResponse>, IResponseBinder
    {
        public Task BindAsync(TResponse responseData, HttpResponse response)
        {
            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return BindAsync(responseData, response, HttpMethods.IsHead(response.HttpContext.Request.Method));
        }

        async Task IResponseBinder.BindAsync(object responseData, HttpResponse response)
        {
            await ((IResponseBinder<TResponse>)this).BindAsync((TResponse)responseData, response);

            var cacheControl = response.GetTypedHeaders().CacheControl;
            if (cacheControl == null)
            {
                cacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { NoCache = true, NoStore = true };
                response.GetTypedHeaders().CacheControl = cacheControl;
            }
        }

        protected abstract Task BindAsync(TResponse responseData, HttpResponse response, bool headerOnly);
    }
}