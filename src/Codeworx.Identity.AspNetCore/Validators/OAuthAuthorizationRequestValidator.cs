using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Validation;

namespace Codeworx.Identity.AspNetCore.Validators
{
    public class OAuthAuthorizationRequestValidator : IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public IValidationResult<AuthorizationErrorResponse> IsValid(AuthorizationRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) {MemberName = nameof(request.ClientId)}, new List<ValidationResult>()))
            {
                return new ClientIdInvalidResult();
            }

            if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) {MemberName = nameof(request.RedirectUri)}, new List<ValidationResult>()))
            {
                return new RedirectUriInvalidResult();
            }

            if (!Validator.TryValidateProperty(request.ResponseType, new ValidationContext(request) {MemberName = nameof(request.ResponseType)}, new List<ValidationResult>()))
            {
                return new ResponseTypeInvalidResult();
            }

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) {MemberName = nameof(request.Scope)}, new List<ValidationResult>()))
            {
                return new ScopeInvalidResult();
            }

            if (!Validator.TryValidateProperty(request.State, new ValidationContext(request) {MemberName = nameof(request.State)}, new List<ValidationResult>()))
            {
                return new StateInvalidResult();
            }

            return null;
        }
    }
}
