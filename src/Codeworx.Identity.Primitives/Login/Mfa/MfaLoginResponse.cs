using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginResponse : IViewData
    {
        public MfaLoginResponse(
            ILoginRegistrationInfo info,
            string providerSelectUrl,
            string cancelUrl,
            string returnUrl)
        {
            Info = info;
            ProviderSelectUrl = providerSelectUrl;
            ReturnUrl = returnUrl;
            CancelUrl = cancelUrl;
        }

        public ILoginRegistrationInfo Info { get; }

        public string ProviderSelectUrl { get; }

        public string CancelUrl { get; }

        public string ReturnUrl { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(ReturnUrl), ReturnUrl);
            target.Add(nameof(Info), Info);
            target.Add(nameof(ProviderSelectUrl), ProviderSelectUrl);
            target.Add(nameof(CancelUrl), CancelUrl);
        }
    }
}