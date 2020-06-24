using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class LoggedinResponse : IViewData
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

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(User), User);
            target.Add(nameof(Providers), Providers);
            target.Add(nameof(ReturnUrl), ReturnUrl);
        }
    }
}