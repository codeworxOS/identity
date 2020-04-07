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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Dictionary<string, IReadOnlyCollection<string>> dictionary;
            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync()
                                        .ConfigureAwait(false);
                dictionary = form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>);
            }
            else
            {
                dictionary = request.Query.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>);
            }

            dictionary.TryGetValue(Identity.OAuth.Constants.ClientIdName, out var clientId);
            dictionary.TryGetValue(Identity.OAuth.Constants.RedirectUriName, out var redirectUri);
            dictionary.TryGetValue(Identity.OAuth.Constants.ResponseTypeName, out var responseType);
            dictionary.TryGetValue(Identity.OAuth.Constants.ScopeName, out var scope);
            dictionary.TryGetValue(Identity.OAuth.Constants.StateName, out var state);
            dictionary.TryGetValue(Identity.OAuth.Constants.NonceName, out var nonce);
            dictionary.TryGetValue(Identity.OAuth.Constants.ResponseModeName, out var responseMode);

            if (clientId?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.ClientIdName, state?.FirstOrDefault());
            }

            if (redirectUri?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.RedirectUriName, state?.FirstOrDefault());
            }

            if (responseType?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.ResponseTypeName, state?.FirstOrDefault());
            }

            if (scope?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.ScopeName, state?.FirstOrDefault());
            }

            if (state?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.StateName, state.FirstOrDefault());
            }

            if (nonce?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.NonceName, state?.FirstOrDefault());
            }

            if (responseMode?.Count > 1)
            {
                throw this.GetErrorResponse(Identity.OAuth.Constants.ResponseModeName, state?.FirstOrDefault());
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