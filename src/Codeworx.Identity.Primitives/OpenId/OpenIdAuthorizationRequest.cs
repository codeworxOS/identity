using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OpenId
{
    [DataContract]
    public class OpenIdAuthorizationRequest : OAuthAuthorizationRequest
    {
        public OpenIdAuthorizationRequest(string clientId, string redirectUri, string responseType, string scope, string state, string nonce)
            : base(clientId, redirectUri, responseType, scope, state, nonce)
        {
            this.Nonce = nonce;
        }

        [Required]
        [UriAbsolute]
        [RegularExpression(OAuth.Constants.RedirectUriValidation)]
        [DataMember(Order = 2, Name = OAuth.Constants.RedirectUriName)]
        public override string RedirectUri { get; }

        [Required]
        [RegularExpression(OAuth.Constants.ScopeValidation)]
        [DataMember(Order = 4, Name = OAuth.Constants.ScopeName)]
        public override string Scope { get; }

        [Required]
        [RegularExpression(OAuth.Constants.NonceValidation)]
        [DataMember(Order = 6, Name = OAuth.Constants.NonceName)]
        public override string Nonce { get; }
    }
}