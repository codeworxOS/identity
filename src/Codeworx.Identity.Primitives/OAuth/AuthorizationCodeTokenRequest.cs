using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        public AuthorizationCodeTokenRequest(string clientId, string redirectUri, string code, string grantType)
            : base(clientId, redirectUri, grantType)
        {
            this.Code = code;
        }

        [Required]
        [RegularExpression(Constants.CodeValidation)]
        [DataMember(Order = 1, Name = Constants.CodeName)]
        public string Code { get; }
    }
}