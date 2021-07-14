using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public class RefreshTokenRequest : TokenRequest
    {
        public RefreshTokenRequest(string clientId, string clientSecret, string refreshToken, string scope = null)
            : base(clientId, Constants.OAuth.GrantType.RefreshToken, clientSecret)
        {
            RefreshToken = refreshToken;
            Scope = scope;
        }

        [Required]
        [RegularExpression(Constants.OAuth.RefreshTokenValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.RefreshTokenName)]
        public string RefreshToken { get; }

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ScopeName)]
        public string Scope { get; }
    }
}