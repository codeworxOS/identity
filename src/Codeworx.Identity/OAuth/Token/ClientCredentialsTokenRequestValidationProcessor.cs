using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsTokenRequestValidationProcessor : IIdentityRequestProcessor<IClientCredentialsParameters, ClientCredentialsTokenRequest>
    {
        public int SortOrder => 150;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IClientCredentialsParameters> builder, ClientCredentialsTokenRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }

            if (!Validator.TryValidateProperty(request.GrantType, new ValidationContext(request) { MemberName = nameof(request.GrantType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.GrantTypeName);
            }

            if (!Validator.TryValidateProperty(request.ClientSecret, new ValidationContext(request) { MemberName = nameof(request.ClientSecret) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientSecretName);
            }

            if (!Validator.TryValidateProperty(request.Scope, new ValidationContext(request) { MemberName = nameof(request.Scope) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ScopeName);
            }
            else
            {
                builder.WithScopes(request.Scope.Split(' '));
            }

            return Task.CompletedTask;
        }
    }
}
