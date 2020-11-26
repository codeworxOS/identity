using System;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginConfiguration
    {
        public OAuthLoginConfiguration()
        {
            IdentifierClaim = Constants.Claims.Subject;
            RedirectCacheMethod = RedirectCacheMethod.UseState;
        }

        public RedirectCacheMethod RedirectCacheMethod { get; set; }

        public Uri BaseUri { get; set; }

        public string IdentifierClaim { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string TokenEndpoint { get; set; }

        public string ClientSecret { get; set; }
    }
}