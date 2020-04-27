using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationTokenResponse : AuthorizationResponse
    {
        public AuthorizationTokenResponse(string state, string token, int expiresIn, string redirectUri)
            : base(state, redirectUri)
        {
            this.Token = token;
            this.ExpiresIn = expiresIn;
        }

        [Required]
        [DataMember(Order = 2, Name = Constants.OAuth.ExpiresInName)]
        public int ExpiresIn { get; }

        [Required]
        [DataMember(Order = 1, Name = Constants.OAuth.AccessTokenName)]
        public string Token { get; }
    }
}