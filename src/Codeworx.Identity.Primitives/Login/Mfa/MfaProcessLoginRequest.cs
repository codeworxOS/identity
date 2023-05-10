using System.Security.Claims;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProcessLoginRequest : MfaLoginRequest
    {
        public MfaProcessLoginRequest(string providerId, object providerRequestParameter, ClaimsIdentity identity, bool headerOnly, string returnUrl, bool noNav)
            : base(identity, headerOnly, providerId, returnUrl, noNav)
        {
            ProviderRequestParameter = providerRequestParameter;
        }

        public object ProviderRequestParameter { get; }
    }
}
