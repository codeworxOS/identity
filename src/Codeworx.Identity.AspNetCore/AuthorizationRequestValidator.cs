using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore
{
    public abstract class AuthorizationRequestValidator<TRequest, TResult> : IRequestValidator<TRequest, TResult>
        where TRequest : OAuthAuthorizationRequest
    {
        private readonly IClientService _clientService;

        protected AuthorizationRequestValidator(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IValidationResult<TResult>> IsValid(TRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.ClientIdName, request.State);
            }

            var client = await _clientService.GetById(request.ClientId)
                .ConfigureAwait(false);
            if (client == null)
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.ClientIdName, request.State);
            }

            if (request.RedirectUri != null)
            {
                if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) { MemberName = nameof(request.RedirectUri) }, new List<ValidationResult>()))
                {
                    return this.GetInvalidResult(Identity.OAuth.Constants.RedirectUriName, request.State);
                }
            }

            if (string.IsNullOrWhiteSpace(request.RedirectUri))
            {
                if (client.DefaultRedirectUri == null)
                {
                    return this.GetInvalidResult(Identity.OAuth.Constants.RedirectUriName, request.State);
                }

                request.RedirectionTarget = client.DefaultRedirectUri.ToString();
            }

            if (!client.ValidRedirectUrls.Any(p => p.Equals(request.RedirectionTarget)))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.RedirectUriName, request.State);
            }

            if (!Validator.TryValidateProperty(request.ResponseType, new ValidationContext(request) { MemberName = nameof(request.ResponseType) }, new List<ValidationResult>()))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.ResponseTypeName, request.State, request.RedirectionTarget);
            }

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) { MemberName = nameof(request.Scope) }, new List<ValidationResult>()))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.ScopeName, request.State, request.RedirectionTarget, Identity.OAuth.Constants.Error.InvalidScope);
            }

            if (!Validator.TryValidateProperty(request.State, new ValidationContext(request) { MemberName = nameof(request.State) }, new List<ValidationResult>()))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.StateName, request.State, request.RedirectionTarget);
            }

            if (!Validator.TryValidateProperty(request.Nonce, new ValidationContext(request) { MemberName = nameof(request.Nonce) }, new List<ValidationResult>()))
            {
                return this.GetInvalidResult(Identity.OAuth.Constants.NonceName, request.State, request.RedirectionTarget);
            }

            return null;
        }

        protected abstract IValidationResult<TResult> GetInvalidResult(string errorDescription, string state, string redirectUri = null, string error = Identity.OAuth.Constants.Error.InvalidRequest);
    }
}