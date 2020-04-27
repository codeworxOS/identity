using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class TokenRequest
    {
        protected TokenRequest(string clientId, string redirectUri, string grantType, string clientSecret)
        {
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.GrantType = grantType;
            this.ClientSecret = clientSecret;
        }

        [RegularExpression(Constants.OAuth.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ClientIdName)]
        public string ClientId { get; }

        [Required]
        [RegularExpression(Constants.OAuth.GrantTypeValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.GrantTypeName)]
        public string GrantType { get; }

        [RegularExpression(Constants.OAuth.RedirectUriValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.RedirectUriName)]
        public string RedirectUri { get; }

        [RegularExpression(Constants.OAuth.ClientSecretValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.ClientSecretName)]
        public string ClientSecret { get; }
    }
}