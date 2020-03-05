using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationCodeResponse : AuthorizationResponse
    {
        public AuthorizationCodeResponse(string state, string code, string redirectUri)
            : base(state, redirectUri)
        {
            this.Code = code;
        }

        [Required]
        [RegularExpression(Constants.CodeValidation)]
        [DataMember(Order = 1, Name = Constants.CodeName)]
        public string Code { get; }
    }
}