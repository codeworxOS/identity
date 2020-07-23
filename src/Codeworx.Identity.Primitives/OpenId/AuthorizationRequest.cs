using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OpenId
{
    [DataContract]
    public class AuthorizationRequest : OAuth.AuthorizationRequest
    {
        public AuthorizationRequest(string clientId, string redirectUri, string responseType, string scope, string state, string nonce, string responseMode, string prompt)
            : base(clientId, redirectUri, responseType, scope, state, nonce, responseMode, prompt)
        {
        }

        [Required]
        [UriAbsolute]
        [RegularExpression(Constants.OAuth.RedirectUriValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.RedirectUriName)]
        public override string RedirectUri => base.RedirectUri;

        [Required]
        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.OAuth.ScopeName)]
        public override string Scope => base.Scope;

        // TODO Check with specs: seams to be optional [Required]
        [RegularExpression(Constants.OAuth.NonceValidation)]
        [DataMember(Order = 6, Name = Constants.OAuth.NonceName)]
        public override string Nonce => base.Nonce;

        public override string GetRequestPath() => "openid";
    }
}