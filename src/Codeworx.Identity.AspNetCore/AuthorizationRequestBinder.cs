using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public abstract class AuthorizationRequestBinder<TRequest> : IRequestBinder<TRequest>
    {
        public async Task<TRequest> BindAsync(HttpRequest request)
        {
            await Task.Yield();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Dictionary<string, IReadOnlyCollection<string>> dictionary;

            // TODO reimplemnet Post Support

            ////if (request.HasFormContentType)
            ////{
            ////    var form = await request.ReadFormAsync()
            ////                            .ConfigureAwait(false);
            ////    dictionary = form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>);
            ////}
            ////else
            ////{
            dictionary = request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>);
            ////}

            dictionary.TryGetValue(Constants.OAuth.ClientIdName, out var clientId);
            dictionary.TryGetValue(Constants.OAuth.RedirectUriName, out var redirectUri);
            dictionary.TryGetValue(Constants.OAuth.ResponseTypeName, out var responseType);
            dictionary.TryGetValue(Constants.OAuth.ScopeName, out var scope);
            dictionary.TryGetValue(Constants.OAuth.StateName, out var state);
            dictionary.TryGetValue(Constants.OAuth.NonceName, out var nonce);
            dictionary.TryGetValue(Constants.OAuth.ResponseModeName, out var responseMode);

            if (clientId?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.ClientIdName, state?.FirstOrDefault());
            }

            if (redirectUri?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.RedirectUriName, state?.FirstOrDefault());
            }

            if (responseType?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.ResponseTypeName, state?.FirstOrDefault());
            }

            if (scope?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.ScopeName, state?.FirstOrDefault());
            }

            if (state?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.StateName, state.FirstOrDefault());
            }

            if (nonce?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.NonceName, state?.FirstOrDefault());
            }

            if (responseMode?.Count > 1)
            {
                throw this.GetErrorResponse(Constants.OAuth.ResponseModeName, state?.FirstOrDefault());
            }

            return this.GetResult(
                clientId?.FirstOrDefault(),
                redirectUri?.FirstOrDefault(),
                responseType?.FirstOrDefault(),
                scope?.FirstOrDefault(),
                state?.FirstOrDefault(),
                nonce?.FirstOrDefault(),
                responseMode?.FirstOrDefault());
        }

        protected abstract ErrorResponseException GetErrorResponse(string errorDescription, string state);

        protected abstract TRequest GetResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null);
    }
}