using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class LoggedinResponse
    {
        public LoggedinResponse(IUser user, IEnumerable<ExternalProviderInfo> providers, string returnUrl = null)
        {
            Providers = providers.ToImmutableList();
            ReturnUrl = returnUrl;
            User = user;
        }

        public IUser User { get; }

        public IEnumerable<ExternalProviderInfo> Providers { get; }

        public string ReturnUrl { get; }
    }
}