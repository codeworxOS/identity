using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Codeworx.Identity.Validation;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string requestPath, string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.ResponseType = responseType;
            this.Scope = scope;
            this.State = state;
            this.Nonce = nonce;
            this.ResponseMode = responseMode;
        }

        [Required]
        [RegularExpression(Constants.OAuth.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ClientIdName)]
        public string ClientId { get; }

        [RegularExpression(Constants.OAuth.NonceValidation)]
        [DataMember(Order = 6, Name = Constants.OAuth.NonceName)]
        public virtual string Nonce { get; }

        [UriAbsolute]
        [RegularExpression(Constants.OAuth.RedirectUriValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.RedirectUriName)]
        public virtual string RedirectUri { get; }

        [RegularExpression(Constants.OAuth.ResponseModeValidation)]
        [DataMember(Order = 7, Name = Constants.OAuth.ResponseModeName)]
        public string ResponseMode { get; }

        [Required]
        [RegularExpression(Constants.OAuth.ResponseTypeValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.ResponseTypeName)]
        public string ResponseType { get; }

        [RegularExpression(Constants.OAuth.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.OAuth.ScopeName)]
        public virtual string Scope { get; }

        [RegularExpression(Constants.OAuth.StateValidation)]
        [DataMember(Order = 5, Name = Constants.OAuth.StateName)]
        public string State { get; }

        public void Append(UriBuilder uriBuilder)
        {
            if (!string.IsNullOrEmpty(ClientId))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.ClientIdName, ClientId);
            }

            if (!string.IsNullOrEmpty(RedirectUri))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.RedirectUriName, RedirectUri);
            }

            if (!string.IsNullOrEmpty(ResponseType))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.ResponseTypeName, ResponseType);
            }

            if (!string.IsNullOrEmpty(Scope))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.ScopeName, Scope);
            }

            if (!string.IsNullOrEmpty(State))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.StateName, State);
            }

            if (!string.IsNullOrEmpty(Nonce))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.NonceName, Nonce);
            }

            if (!string.IsNullOrEmpty(ResponseMode))
            {
                uriBuilder.AppendQueryPart(Constants.OAuth.ResponseModeName, ResponseMode);
            }
        }
    }
}