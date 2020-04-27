using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        public AuthorizationCodeTokenRequest(string clientId, string redirectUri, string code, string grantType, string clientSecret)
            : base(clientId, redirectUri, grantType, clientSecret)
        {
            this.Code = code;
        }

        [Required]
        [RegularExpression(Constants.OAuth.CodeValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.CodeName)]
        public string Code { get; }
    }
}