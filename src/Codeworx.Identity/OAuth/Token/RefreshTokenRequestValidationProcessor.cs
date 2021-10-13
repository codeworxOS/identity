using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenRequestValidationProcessor : IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>
    {
        public int SortOrder => 150;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IRefreshTokenParameters> builder, RefreshTokenRequest request)
        {
            var error = false;

            if (!Validator.TryValidateProperty(request.RefreshToken, new ValidationContext(request) { MemberName = nameof(request.RefreshToken) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.GrantType, new ValidationContext(request) { MemberName = nameof(request.GrantType) }, new List<ValidationResult>()))
            {
                error = error || true;
            }

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) { MemberName = nameof(request.Scope) }, new List<ValidationResult>()))
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

            builder.WithRefreshToken(request.RefreshToken)
                .WithScopes(request.Scope?.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries) ?? new string[] { });

            return Task.CompletedTask;
        }
    }
}
