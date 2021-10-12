using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth.Token
{
    [DataContract]
    public class TokenExchangeRequest : TokenRequest
    {
        public TokenExchangeRequest(string clientId, string clientSecret, string audience, string scope)
            : base(clientSecret, Constants.OAuth.GrantType.RefreshToken, clientSecret)
        {
            Audience = audience;
            Scope = scope;
        }

        [RegularExpression(Constants.OAuth.AudienceValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.AudienceName)]
        public string Audience { get; set; }

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.ScopeName)]
        public string Scope { get; set; }

        ////subject_token
        ////   REQUIRED.A security token that represents the identity of the     
        ////   party on behalf of whom the request is being made.  Typically, the
        ////   subject of this token will be the subject of the security token
        ////   issued in response to the request.

        ////subject_token_type
        ////   REQUIRED.  An identifier, as described in Section 3, that
        ////   indicates the type of the security token in the "subject_token"

        ////   parameter.

        ////actor_token
        ////   OPTIONAL.  A security token that represents the identity of the
        ////   acting party.Typically, this will be the party that is

        ////   authorized to use the requested security token and act on behalf
        ////   of the subject.

        ////actor_token_type
        ////   An identifier, as described in Section 3, that indicates the type
        ////   of the security token in the "actor_token" parameter.This is
        ////   REQUIRED when the "actor_token" parameter is present in the
        ////   request but MUST NOT be included otherwise.
    }
}