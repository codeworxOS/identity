using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.AspNetCore
{
    public abstract class AuthorizationRequestBinder<TRequest, TError> : IRequestBinder<TRequest, TError>
    {
        public IRequestBindingResult<TRequest, TError> FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query)
        {
            IReadOnlyCollection<string> clientId = null;
            IReadOnlyCollection<string> redirectUri = null;
            IReadOnlyCollection<string> responseType = null;
            IReadOnlyCollection<string> scope = null;
            IReadOnlyCollection<string> state = null;
            IReadOnlyCollection<string> nonce = null;

            query?.TryGetValue(Identity.OAuth.Constants.ClientIdName, out clientId);
            query?.TryGetValue(Identity.OAuth.Constants.RedirectUriName, out redirectUri);
            query?.TryGetValue(Identity.OAuth.Constants.ResponseTypeName, out responseType);
            query?.TryGetValue(Identity.OAuth.Constants.ScopeName, out scope);
            query?.TryGetValue(Identity.OAuth.Constants.StateName, out state);
            query?.TryGetValue(Identity.OAuth.Constants.NonceName, out nonce);

            if (clientId?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.ClientIdName, state?.FirstOrDefault());
            }

            if (redirectUri?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.RedirectUriName, state?.FirstOrDefault());
            }

            if (responseType?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.ResponseTypeName, state?.FirstOrDefault());
            }

            if (scope?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.ScopeName, state?.FirstOrDefault());
            }

            if (state?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.StateName, state?.FirstOrDefault());
            }

            if (nonce?.Count > 1)
            {
                return this.GetErrorResult(Identity.OAuth.Constants.NonceName, state?.FirstOrDefault());
            }

            return this.GetSuccessfulResult(
                clientId?.FirstOrDefault(),
                redirectUri?.FirstOrDefault(),
                responseType?.FirstOrDefault(),
                scope?.FirstOrDefault(),
                state?.FirstOrDefault(),
                nonce?.FirstOrDefault());
        }

        protected abstract IRequestBindingResult<TRequest, TError> GetErrorResult(string errorDescription, string state);

        protected abstract IRequestBindingResult<TRequest, TError> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce);
    }
}