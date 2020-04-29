using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class StageTwoAuthorizationRequestProcessor : IAuthorizationRequestProcessor
    {
        public Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request)
        {
            var parameters = builder.Parameters;

            if (!Validator.TryValidateProperty(request.ResponseType, new ValidationContext(request) { MemberName = nameof(request.ResponseType) }, new List<ValidationResult>()))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ResponseTypeName, request.State, parameters.RedirectUri);
            }

            builder = builder.WithResponseTypes(request.ResponseType?.Split(' ') ?? new string[] { });

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) { MemberName = nameof(request.Scope) }, new List<ValidationResult>()))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ScopeName, request.State, parameters.RedirectUri, Constants.OAuth.Error.InvalidScope);
            }

            builder = builder.WithScopes(request.Scope?.Split(' ') ?? new string[] { });

            if (!Validator.TryValidateProperty(request.Nonce, new ValidationContext(request) { MemberName = nameof(request.Nonce) }, new List<ValidationResult>()))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.NonceName, request.State, parameters.RedirectUri);
            }

            builder = builder.WithNonce(request.Nonce);

            return Task.FromResult(builder);
        }
    }
}
