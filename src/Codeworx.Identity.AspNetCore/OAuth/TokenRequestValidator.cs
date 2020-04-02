using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Validation.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenRequestValidator : IRequestValidator<TokenRequest, ErrorResponse>
    {
        public Task<IValidationResult<ErrorResponse>> IsValid(TokenRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                return Task.FromResult<IValidationResult<ErrorResponse>>(new ClientIdInvalidResult());
            }

            if (!Validator.TryValidateProperty(request.GrantType, new ValidationContext(request) { MemberName = nameof(request.GrantType) }, new List<ValidationResult>()))
            {
                return Task.FromResult<IValidationResult<ErrorResponse>>(new GrantTypeInvalidResult());
            }

            if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) { MemberName = nameof(request.RedirectUri) }, new List<ValidationResult>()))
            {
                return Task.FromResult<IValidationResult<ErrorResponse>>(new RedirectUriInvalidResult());
            }

            if (!Validator.TryValidateProperty(request.ClientSecret, new ValidationContext(request) { MemberName = nameof(request.ClientSecret) }, new List<ValidationResult>()))
            {
                return Task.FromResult<IValidationResult<ErrorResponse>>(new ClientSecretInvalidResult());
            }

            return Task.FromResult<IValidationResult<ErrorResponse>>(null);
        }
    }
}