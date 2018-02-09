using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class TokenResponse
    {
        [Required]
        [RegularExpression(Constants.AccessTokenValidation)]
        [DataMember(Order = 1, Name = Constants.AccessTokenName)]
        public string AccessToken { get; set; }

        [DataMember(Order = 3, Name = Constants.ExpiresInName)]
        public int ExpiresIn { get; set; }

        [RegularExpression(Constants.RefreshTokenValidation)]
        [DataMember(Order = 4, Name = Constants.RefreshTokenName)]
        public string RefreshToken { get; set; }

        [RegularExpression(Constants.ScopeValidation)]
        [DataMember(Order = 5, Name = Constants.ScopeName)]
        public string Scope { get; set; }

        [Required]
        [RegularExpression(Constants.TokenTypeValidation)]
        [DataMember(Order = 2, Name = Constants.TokenTypeName)]
        public string TokenType { get; set; }
    }
}