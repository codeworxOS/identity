using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginResponse : IViewData
    {
        public MfaLoginResponse(
            ILoginRegistrationInfo info,
            string providerSelectUrl,
            string returnUrl = null)
        {
            Info = info;
            ProviderSelectUrl = providerSelectUrl;
            ReturnUrl = returnUrl;
        }

        public ILoginRegistrationInfo Info { get; }

        public string ProviderSelectUrl { get; }

        public string ReturnUrl { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(ReturnUrl), ReturnUrl);
            target.Add(nameof(Info), Info);
            target.Add(nameof(ProviderSelectUrl), ProviderSelectUrl);
        }
    }
}