using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.Mfa.Totp.Binder
{
    public class TotpLoginRequestBinder : IRequestBinder<TotpLoginRequest>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public TotpLoginRequestBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
        }

        public async Task<TotpLoginRequest> BindAsync(HttpRequest request)
        {
            var auth = await _handler.AuthenticateAsync(request.HttpContext).ConfigureAwait(false);

            if (!auth.Succeeded)
            {
                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            if (request.HasFormContentType)
            {
                string providerId = null;
                string oneTimeCode = null;
                string sharedSecret = null;

                if (request.Form.TryGetValue("provider-id", out var providerIdValues))
                {
                    providerId = providerIdValues;
                }

                if (request.Form.TryGetValue("one-time-code", out var oneTimeCodeValues))
                {
                    oneTimeCode = oneTimeCodeValues;
                }

                if (request.Form.TryGetValue("shared-secret", out var sharedSecretValues))
                {
                    sharedSecret = sharedSecretValues;
                }

                var result = new TotpLoginRequest(providerId, (ClaimsIdentity)auth.Principal.Identity, string.IsNullOrWhiteSpace(sharedSecret) ? TotpAction.Login : TotpAction.Register, oneTimeCode, sharedSecret);

                return result;
            }

            throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
        }
    }
}
