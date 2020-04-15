using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OpenId
{
    [DataContract]
    public class AuthorizationRequest : OAuth.AuthorizationRequest
    {
        public AuthorizationRequest(string clientId, string redirectUri, string responseType, string scope, string state, string nonce, string responseMode)
            : base(clientId, redirectUri, responseType, scope, state, nonce, responseMode)
        {
        }

        [Required]
        [UriAbsolute]
        [RegularExpression(OAuth.Constants.RedirectUriValidation)]
        [DataMember(Order = 2, Name = OAuth.Constants.RedirectUriName)]
        public override string RedirectUri => base.RedirectUri;

        [Required]
        [RegularExpression(OAuth.Constants.ScopeValidation)]
        [DataMember(Order = 4, Name = OAuth.Constants.ScopeName)]
        public override string Scope => base.Scope;

        // TODO Check with specs: seams to be optional [Required]
        [RegularExpression(OAuth.Constants.NonceValidation)]
        [DataMember(Order = 6, Name = OAuth.Constants.NonceName)]
        public override string Nonce => base.Nonce;
    }
}