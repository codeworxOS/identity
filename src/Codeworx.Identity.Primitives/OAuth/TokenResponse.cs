using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class TokenResponse
    {
        public TokenResponse(string accessToken, string identityToken, string tokenType, int expiresIn, string scope, string refreshToken = null)
        {
            this.AccessToken = accessToken;
            this.IdToken = identityToken;
            this.ExpiresIn = expiresIn;
            this.RefreshToken = refreshToken;
            this.Scope = scope;
            this.TokenType = tokenType;
        }

        [Required]
        [RegularExpression(Constants.OAuth.AccessTokenValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.AccessTokenName)]
        public string AccessToken { get; }

        [DataMember(Order = 3, Name = Constants.OAuth.ExpiresInName)]
        public int ExpiresIn { get; }

        [RegularExpression(Constants.OAuth.RefreshTokenValidation)]
        [DataMember(Order = 4, Name = Constants.OAuth.RefreshTokenName)]
        public string RefreshToken { get; }

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 5, Name = Constants.OAuth.ScopeName)]
        public string Scope { get; }

        [Required]
        [RegularExpression(Constants.OAuth.TokenTypeValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.TokenTypeName)]
        public string TokenType { get; }

        [RegularExpression(Constants.OpenId.IdTokenValidation)]
        [DataMember(Order = 6, Name = Constants.OpenId.IdTokenName)]
        public string IdToken { get; }
    }
}