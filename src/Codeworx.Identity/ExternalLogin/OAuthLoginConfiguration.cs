using System;

namespace Codeworx.Identity.ExternalLogin
{
    public class OAuthLoginConfiguration
    {
        public Uri BaseUri { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string TokenEndpoint { get; set; }

        public string ClientSecret { get; set; }
    }
}