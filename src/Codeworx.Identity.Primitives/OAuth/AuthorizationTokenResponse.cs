using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationTokenResponse : AuthorizationResponse
    {
        public AuthorizationTokenResponse(string state, string token, string redirectUri)
            : base(state, redirectUri)
        {
            this.Token = token;
        }

        [Required]
        [DataMember(Order = 1, Name = Constants.AccessTokenName)]
        public string Token { get; }
    }
}