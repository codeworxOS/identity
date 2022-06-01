using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeRequestValidationProcessor : IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>
    {
        public int SortOrder => 150;

        public Task ProcessAsync(IIdentityDataParametersBuilder<ITokenExchangeParameters> builder, TokenExchangeRequest request)
        {
            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }

            if (!Validator.TryValidateProperty(request.GrantType, new ValidationContext(request) { MemberName = nameof(request.GrantType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.GrantTypeName);
            }

            if (request.GrantType != Constants.OAuth.GrantType.TokenExchange)
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

            if (!Validator.TryValidateProperty(request.Audience, new ValidationContext(request) { MemberName = nameof(request.Audience) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.AudienceName);
            }
            else
            {
                builder.WithAudience(request.Audience);
            }

            if (!Validator.TryValidateProperty(request.SubjectToken, new ValidationContext(request) { MemberName = nameof(request.SubjectToken) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.SubjectTokenName);
            }
            else
            {
                builder.WithSubjectToken(request.SubjectToken);
            }

            if (!Validator.TryValidateProperty(request.SubjectTokenType, new ValidationContext(request) { MemberName = nameof(request.SubjectTokenType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.SubjectTokenTypeName);
            }
            else
            {
                builder.WithSubjectTokenType(request.SubjectTokenType);
            }

            if (!Validator.TryValidateProperty(request.ActorToken, new ValidationContext(request) { MemberName = nameof(request.ActorToken) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ActorTokenName);
            }
            else
            {
                builder.WithActorToken(request.ActorToken);
            }

            if (!Validator.TryValidateProperty(request.ActorTokenType, new ValidationContext(request) { MemberName = nameof(request.ActorTokenType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ActorTokenTypeName);
            }
            else
            {
                builder.WithActorTokenType(request.ActorTokenType);
            }

            if (!Validator.TryValidateProperty(request.RequestedTokenType, new ValidationContext(request) { MemberName = nameof(request.RequestedTokenType) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RequestedTokenTypeName);
            }
            else
            {
                builder.WithRequestedTokenTypes(request.RequestedTokenType?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { });
            }

            return Task.CompletedTask;
        }
    }
}
