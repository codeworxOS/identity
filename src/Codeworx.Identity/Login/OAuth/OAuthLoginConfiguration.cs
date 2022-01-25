using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginConfiguration
    {
        public OAuthLoginConfiguration()
        {
            IdentifierClaim = Constants.Claims.Subject;
            RedirectCacheMethod = RedirectCacheMethod.UseState;
            TokenHandling = ExternalTokenHandling.None;
            ClaimSource = ClaimSource.AccessToken;
            ClientAuthenticationMode = ClientAuthenticationMode.Header;
            AuthorizationParameters = new Dictionary<string, object>();
            TokenParameters = new Dictionary<string, object>();
        }

        public ClientAuthenticationMode ClientAuthenticationMode { get; set; }

        public RedirectCacheMethod RedirectCacheMethod { get; set; }

        public Uri BaseUri { get; set; }

        public string IdentifierClaim { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string CssClass { get; set; }

        public ClaimSource ClaimSource { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string TokenEndpoint { get; set; }

        public string ClientSecret { get; set; }

        public ExternalTokenHandling TokenHandling { get; set; }

        public Dictionary<string, object> AuthorizationParameters { get; }

        public Dictionary<string, object> TokenParameters { get; }

        public Uri GetTokenEndpointUri()
        {
            if (Uri.TryCreate(this.TokenEndpoint, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    return uri;
                }
                else
                {
                    var builder = new UriBuilder(BaseUri);
                    builder.AppendPath(this.TokenEndpoint);

                    return new Uri(builder.ToString());
                }
            }

            return BaseUri;
        }

        public Uri GetAuthorizationEndpointUri()
        {
            if (Uri.TryCreate(this.AuthorizationEndpoint, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    return uri;
                }
                else
                {
                    var builder = new UriBuilder(BaseUri);
                    builder.AppendPath(this.AuthorizationEndpoint);

                    return new Uri(builder.ToString());
                }
            }

            return BaseUri;
        }
    }
}