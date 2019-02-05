using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string clientId, string redirectUri, string responseType, string scope, string state)
        {
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.ResponseType = responseType;
            this.Scope = scope;
            this.State = state;
        }

        [Required]
        [RegularExpression(Constants.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.ClientIdName)]
        public string ClientId { get; }

        [Required]
        [RegularExpression(Constants.RedirectUriValidation)]
        [UriAbsolute]
        [DataMember(Order = 2, Name = Constants.RedirectUriName)]
        public string RedirectUri { get; }

        [Required]
        [RegularExpression(Constants.ResponseTypeValidation)]
        [DataMember(Order = 3, Name = Constants.ResponseTypeName)]
        public string ResponseType { get; }

        [RegularExpression(Constants.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.ScopeName)]
        public string Scope { get; }

        [RegularExpression(Constants.StateValidation)]
        [DataMember(Order = 5, Name = Constants.StateName)]
        public string State { get; }
    }
}