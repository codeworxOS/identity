using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class TokenRequest
    {
        protected TokenRequest(string clientId, string redirectUri, string grantType)
        {
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.GrantType = grantType;
        }

        [RegularExpression(Constants.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.ClientIdName)]
        public string ClientId { get; }

        [Required]
        [RegularExpression(Constants.GrantTypeValidation)]
        [DataMember(Order = 2, Name = Constants.GrantTypeName)]
        public string GrantType { get; }

        [RegularExpression(Constants.RedirectUriValidation)]
        [DataMember(Order = 3, Name = Constants.RedirectUriName)]
        public string RedirectUri { get; }
    }
}