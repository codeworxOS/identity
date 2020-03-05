using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class LoggedinResponse
    {
        public LoggedinResponse(IEnumerable<ExternalProviderInfo> providers, string returnUrl = null)
        {
            Providers = providers.ToImmutableList();
            ReturnUrl = returnUrl;
        }

        public IEnumerable<ExternalProviderInfo> Providers { get; }

        public string ReturnUrl { get; }
    }
}