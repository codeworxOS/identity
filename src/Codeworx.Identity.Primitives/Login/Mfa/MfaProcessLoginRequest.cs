using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProcessLoginRequest : MfaLoginRequest
    {
        public MfaProcessLoginRequest(string providerId, object providerRequestParameter, ClaimsIdentity identity, string returnUrl = null)
            : base(identity, returnUrl)
        {
            ProviderId = providerId;
            ProviderRequestParameter = providerRequestParameter;
        }

        public string ProviderId { get; }

        public object ProviderRequestParameter { get; }
    }
}
