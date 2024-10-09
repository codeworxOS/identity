using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public class ClientCredentialsTokenRequest : TokenRequest
    {
        public ClientCredentialsTokenRequest(string clientId, string clientSecret, string scope)
            : base(clientId, Constants.OAuth.GrantType.ClientCredentials, clientSecret)
        {
            Scope = scope;
        }

        public ClientCredentialsTokenRequest(string clientId, string clientSecret, string scope, DateTimeOffset? validUntil)
            : base(clientId, Constants.OAuth.GrantType.ClientCredentials, clientSecret)
        {
            Scope = scope;
            ValidUntil = validUntil;
        }

        [Required]
        [RegularExpression(Constants.OAuth.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ClientIdName)]
        public override string ClientId => base.ClientId;

        [Required]
        [RegularExpression(Constants.OAuth.ClientSecretValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.ClientSecretName)]
        public override string ClientSecret => base.ClientSecret;

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.OAuth.ScopeName)]
        public virtual string Scope { get; }

        public DateTimeOffset? ValidUntil { get; }
    }
}