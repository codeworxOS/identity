using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProcessLoginRequest : MfaLoginRequest
    {
        public MfaProcessLoginRequest(string providerId, object providerRequestParameter, ClaimsIdentity identity, bool headerOnly, string returnUrl = null)
            : base(identity, headerOnly, providerId, returnUrl)
        {
            ProviderRequestParameter = providerRequestParameter;
        }

        public object ProviderRequestParameter { get; }
    }
}
