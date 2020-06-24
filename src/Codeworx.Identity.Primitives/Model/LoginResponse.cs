using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class LoginResponse : IViewData
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

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(Providers), Providers);
            target.Add(nameof(ReturnUrl), ReturnUrl);
            target.Add(nameof(Username), Username);
        }
    }
}