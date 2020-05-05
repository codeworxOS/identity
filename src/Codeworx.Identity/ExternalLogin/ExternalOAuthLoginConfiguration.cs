using System;

namespace Codeworx.Identity.ExternalLogin
{
    public class ExternalOAuthLoginConfiguration
    {
        public ExternalOAuthLoginConfiguration()
        {
            IdentifierClaim = Constants.Claims.Subject;
        }

        public Uri BaseUri { get; set; }

        public string IdentifierClaim { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string TokenEndpoint { get; set; }

        public string ClientSecret { get; set; }
    }
}