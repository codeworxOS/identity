using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenRequestValidator : IRequestValidator<TokenRequest>
    {
        public Task ValidateAsync(TokenRequest request)
        {
            var error = false;

            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.GrantType, new ValidationContext(request) { MemberName = nameof(request.GrantType) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) { MemberName = nameof(request.RedirectUri) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.ClientSecret, new ValidationContext(request) { MemberName = nameof(request.ClientSecret) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (error)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest);
            }

            return Task.CompletedTask;
        }
    }
}
