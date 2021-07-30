using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public abstract class TokenRequest
    {
        protected TokenRequest(string clientId, string grantType, string clientSecret)
        {
            this.ClientId = clientId;
            this.GrantType = grantType;
            this.ClientSecret = clientSecret;
        }

        [RegularExpression(Constants.OAuth.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ClientIdName)]
        public virtual string ClientId { get; }

        [Required]
        [RegularExpression(Constants.OAuth.GrantTypeValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.GrantTypeName)]
        public virtual string GrantType { get; }

        [RegularExpression(Constants.OAuth.ClientSecretValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.ClientSecretName)]
        public virtual string ClientSecret { get; }
    }
}