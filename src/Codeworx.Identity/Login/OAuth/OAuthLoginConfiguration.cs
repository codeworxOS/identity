using System;

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
        }

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
    }
}