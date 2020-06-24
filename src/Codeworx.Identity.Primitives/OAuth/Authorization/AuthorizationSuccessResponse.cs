using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.View;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationSuccessResponse : AuthorizationResponse, IViewData
    {
        public AuthorizationSuccessResponse(string state, string code, string token, int? expiresIn, string identityToken, string redirectUri, string responseMode)
            : base(state, redirectUri)
        {
            this.Code = code;
            this.Token = token;
            this.IdToken = identityToken;
            this.ExpiresIn = expiresIn;
            this.ResponseMode = responseMode;
        }

        [RegularExpression(Constants.OAuth.CodeValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.CodeName)]
        public string Code { get; }

        [IgnoreDataMember]
        public string ResponseMode { get; }

        [DataMember(Order = 2, Name = Constants.OAuth.ExpiresInName)]
        public int? ExpiresIn { get; }

        [DataMember(Order = 3, Name = Constants.OAuth.AccessTokenName)]
        [RegularExpression(Constants.OAuth.AccessTokenValidation)]
        public string Token { get; }

        [DataMember(Order = 4, Name = Constants.OpenId.IdTokenName)]
        [RegularExpression(Constants.OpenId.IdTokenValidation)]
        public string IdToken { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(State), State);
            target.Add(nameof(RedirectUri), RedirectUri);
            target.Add(nameof(Code), Code);
            target.Add(nameof(Token), Token);
            target.Add(nameof(IdToken), IdToken);
            target.Add(nameof(ExpiresIn), ExpiresIn);
            target.Add(nameof(ResponseMode), ResponseMode);
        }
    }
}