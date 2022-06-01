using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public class TokenExchangeRequest : TokenRequest
    {
        public TokenExchangeRequest(string clientId, string clientSecret, string audience, string scope, string subjectToken, string subjectTokenType, string actorToken, string actorTokenType, string requestedTokenType)
            : base(clientId, Constants.OAuth.GrantType.TokenExchange, clientSecret)
        {
            Audience = audience;
            Scope = scope;
            SubjectToken = subjectToken;
            SubjectTokenType = subjectTokenType;
            ActorToken = actorToken;
            ActorTokenType = actorToken;
            RequestedTokenType = requestedTokenType;
        }

        [RegularExpression(Constants.OAuth.AudienceValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.AudienceName)]
        public string Audience { get; set; }

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.ScopeName)]
        public string Scope { get; set; }

        [Required]
        [RegularExpression(Constants.OAuth.SubjectTokenValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.SubjectTokenName)]
        public string SubjectToken { get; set; }

        [Required]
        [RegularExpression(Constants.OAuth.SubjectTokenTypeValidation)]
        [DataMember(Order = 4, Name = Constants.OAuth.SubjectTokenTypeName)]
        public string SubjectTokenType { get; set; }

        [RegularExpression(Constants.OAuth.ActorTokenValidation)]
        [DataMember(Order = 5, Name = Constants.OAuth.ActorTokenName)]
        public string ActorToken { get; set; }

        [RegularExpression(Constants.OAuth.ActorTokenTypeValidation)]
        [DataMember(Order = 6, Name = Constants.OAuth.ActorTokenTypeName)]
        public string ActorTokenType { get; set; }

        [RegularExpression(Constants.OAuth.RequestedTokenTypeValidation)]
        [DataMember(Order = 7, Name = Constants.OAuth.RequestedTokenTypeName)]
        public string RequestedTokenType { get; set; }
    }
}