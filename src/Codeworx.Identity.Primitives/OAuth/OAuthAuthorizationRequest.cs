using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class OAuthAuthorizationRequest
    {
        private string _defaultRedirectUri;

        public OAuthAuthorizationRequest(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.ResponseType = responseType;
            this.Scope = scope;
            this.State = state;
            this.Nonce = nonce;
            this.ResponseMode = responseMode;
        }

        [Required]
        [RegularExpression(Constants.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.ClientIdName)]
        public string ClientId { get; }

        [UriAbsolute]
        [RegularExpression(Constants.RedirectUriValidation)]
        [DataMember(Order = 2, Name = Constants.RedirectUriName)]
        public virtual string RedirectUri { get; }

        [Required]
        [RegularExpression(Constants.ResponseTypeValidation)]
        [DataMember(Order = 3, Name = Constants.ResponseTypeName)]
        public string ResponseType { get; }

        [RegularExpression(Constants.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.ScopeName)]
        public virtual string Scope { get; }

        [RegularExpression(Constants.StateValidation)]
        [DataMember(Order = 5, Name = Constants.StateName)]
        public string State { get; }

        [RegularExpression(Constants.NonceValidation)]
        [DataMember(Order = 6, Name = Constants.NonceName)]
        public virtual string Nonce { get; }

        [RegularExpression(Constants.ResponseModeValidation)]
        [DataMember(Order = 7, Name = OAuth.Constants.ResponseModeName)]
        public string ResponseMode { get; }

        [IgnoreDataMember]
        public string RedirectionTarget
        {
            get => string.IsNullOrWhiteSpace(_defaultRedirectUri) ? this.RedirectUri : _defaultRedirectUri;
            set => _defaultRedirectUri = value;
        }
    }
}