using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class LoginResponse
    {
        public LoginResponse(IEnumerable<ExternalProviderInfo> providers, string returnUrl = null, string username = null, string error = null)
        {
            Providers = providers.ToImmutableList();
            ReturnUrl = returnUrl;
            Username = username;
            Error = error;
        }

        public string Error { get; }

        public IEnumerable<ExternalProviderInfo> Providers { get; }

        public string ReturnUrl { get; }

        public string Username { get; }
    }
}