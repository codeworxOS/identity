using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Validation.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationRequestValidator : IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>
    {
        private readonly IClientService _clientService;

        public AuthorizationRequestValidator(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IValidationResult<AuthorizationErrorResponse>> IsValid(AuthorizationRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) {MemberName = nameof(request.ClientId)}, new List<ValidationResult>()))
            {
                return new ClientIdInvalidResult(request.State);
            }

            var client = await _clientService.GetById(request.ClientId)
                                             .ConfigureAwait(false);
            if (client == null)
            {
                return new ClientIdInvalidResult(request.State);
            }

            if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) {MemberName = nameof(request.RedirectUri)}, new List<ValidationResult>()))
            {
                return new RedirectUriInvalidResult(request.State);
            }

            if (string.IsNullOrWhiteSpace(request.RedirectUri))
            {
                if (client.DefaultRedirectUri == null)
                {
                    return new RedirectUriInvalidResult(request.State);
                }

                request.RedirectionTarget = client.DefaultRedirectUri.ToString();
            }

            if (!client.ValidRedirectUrls.Contains(request.RedirectionTarget))
            {
                return new RedirectUriInvalidResult(request.State);
            }

            if (!Validator.TryValidateProperty(request.ResponseType, new ValidationContext(request) {MemberName = nameof(request.ResponseType)}, new List<ValidationResult>()))
            {
                return new ResponseTypeInvalidResult(request.RedirectionTarget, request.State);
            }

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) {MemberName = nameof(request.Scope)}, new List<ValidationResult>()))
            {
                return new ScopeInvalidResult(request.RedirectionTarget, request.State);
            }

            if (!Validator.TryValidateProperty(request.State, new ValidationContext(request) {MemberName = nameof(request.State)}, new List<ValidationResult>()))
            {
                return new StateInvalidResult(request.RedirectionTarget, request.State);
            }

            return null;
        }
    }
}
