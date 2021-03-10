using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class StageTwoAuthorizationRequestProcessor : IIdentityRequestProcessor<IAuthorizationParameters, AuthorizationRequest>
    {
        public int SortOrder => 200;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IAuthorizationParameters> builder, AuthorizationRequest request)
        {
            var parameters = builder.Parameters;

            if (!Validator.TryValidateProperty(request.ResponseType, new ValidationContext(request) { MemberName = nameof(request.ResponseType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ResponseTypeName);
            }

            builder = builder.WithResponseTypes(request.ResponseType?.Split(' ') ?? new string[] { });

            parameters = builder.Parameters;
            var defaultResponseMode = parameters.ResponseTypes.Count == 1 && parameters.ResponseTypes.Contains(Constants.OAuth.ResponseType.Code) ?
                                        Constants.OAuth.ResponseMode.Query :
                                        Constants.OAuth.ResponseMode.Fragment;

            builder.WithResponseMode(defaultResponseMode);

            if (!Validator.TryValidateProperty(request.ResponseMode, new ValidationContext(request) { MemberName = nameof(request.ResponseMode) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ResponseModeName);
            }

            if (request.ResponseMode == Constants.OAuth.ResponseMode.Query &&
                (request.ResponseType.Contains(Constants.OAuth.ResponseType.Token) || request.ResponseType.Contains(Constants.OpenId.ResponseType.IdToken)))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ResponseModeName);
            }

            builder.WithResponseMode(request.ResponseMode ?? defaultResponseMode);

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) { MemberName = nameof(request.Scope) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ScopeName);
            }

            builder = builder.WithScopes(request.Scope?.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries) ?? new string[] { });

            if (!Validator.TryValidateProperty(request.Nonce, new ValidationContext(request) { MemberName = nameof(request.Nonce) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.NonceName);
            }

            builder = builder.WithNonce(request.Nonce);

            return Task.CompletedTask;
        }
    }
}
