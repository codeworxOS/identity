using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Mfa.Mail;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa.Mail
{
    public class MailLoginRequestBinder : IRequestBinder<MailLoginRequest>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public MailLoginRequestBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
        }

        public async Task<MailLoginRequest> BindAsync(HttpRequest request)
        {
            var auth = await _handler.AuthenticateAsync(request.HttpContext).ConfigureAwait(false);

            if (!auth.Succeeded)
            {
                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            if (request.HasFormContentType)
            {
                string providerId = null;
                string oneTimeCode = string.Empty;
                string email = null;
                string returnUrl = null;
                string sessionId = null;

                if (request.Form.TryGetValue("provider-id", out var providerIdValues))
                {
                    providerId = providerIdValues;
                }

                if (request.Form.TryGetValue("email", out var emailValues))
                {
                    email = emailValues;
                }

                if (request.Form.TryGetValue("session-id", out var sessionIdValues))
                {
                    sessionId = sessionIdValues;
                }

                foreach (var item in request.Form.Where(p => p.Key.StartsWith("code-")).OrderBy(p => p.Key))
                {
                    oneTimeCode += item.Value;
                }

                if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
                {
                    returnUrl = returnUrlValues;
                }

                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    return new RegisterMailLoginRequest(providerId, (ClaimsIdentity)auth.Principal.Identity, returnUrl, email, sessionId, oneTimeCode, auth.Properties.IsPersistent);
                }

                return new ProcessMailLoginRequest(providerId, (ClaimsIdentity)auth.Principal.Identity, returnUrl, oneTimeCode, auth.Properties.IsPersistent);
            }

            throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
        }
    }
}
