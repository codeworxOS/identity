using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Model
{
    public class ProviderInfosResponse
    {
        public ProviderInfosResponse(IEnumerable<ExternalProviderInfo> providers)
        {
            Providers = providers.ToImmutableList();
        }

        public IEnumerable<ExternalProviderInfo> Providers { get; }
    }
}