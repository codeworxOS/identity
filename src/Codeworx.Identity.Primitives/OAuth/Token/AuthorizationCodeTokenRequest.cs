using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        public AuthorizationCodeTokenRequest(string clientId, string redirectUri, string code, string clientSecret)
            : base(clientId, Constants.OAuth.GrantType.AuthorizationCode, clientSecret)
        {
            this.Code = code;
            RedirectUri = redirectUri;
        }

        [RegularExpression(Constants.OAuth.RedirectUriValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.RedirectUriName)]
        public virtual string RedirectUri { get; }

        [Required]
        [RegularExpression(Constants.OAuth.CodeValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.CodeName)]
        public string Code { get; }
    }
}