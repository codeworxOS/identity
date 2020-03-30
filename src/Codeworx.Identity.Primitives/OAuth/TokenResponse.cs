using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class TokenResponse
    {
        public TokenResponse(string accessToken, string idToken, string tokenType, int expiresIn = 0, string refreshToken = null, string scope = null)
        {
            this.AccessToken = accessToken;
            this.IdToken = idToken;
            this.ExpiresIn = expiresIn;
            this.RefreshToken = refreshToken;
            this.Scope = scope;
            this.TokenType = tokenType;
        }

        [Required]
        [RegularExpression(Constants.AccessTokenValidation)]
        [DataMember(Order = 1, Name = Constants.AccessTokenName)]
        public string AccessToken { get; }

        [DataMember(Order = 3, Name = Constants.ExpiresInName)]
        public int ExpiresIn { get; }

        [RegularExpression(Constants.RefreshTokenValidation)]
        [DataMember(Order = 4, Name = Constants.RefreshTokenName)]
        public string RefreshToken { get; }

        [RegularExpression(Constants.ScopeValidation)]
        [DataMember(Order = 5, Name = Constants.ScopeName)]
        public string Scope { get; }

        [Required]
        [RegularExpression(Constants.TokenTypeValidation)]
        [DataMember(Order = 2, Name = Constants.TokenTypeName)]
        public string TokenType { get; }

        [RegularExpression(Constants.IdTokenValidation)]
        [DataMember(Order = 6, Name = Constants.IdTokenName)]
        public string IdToken { get; }
    }
}