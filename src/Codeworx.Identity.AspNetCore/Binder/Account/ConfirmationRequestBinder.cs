using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ConfirmationRequestBinder : IRequestBinder<ConfirmationRequest>
    {
        private readonly IdentityServerOptions _options;
        private readonly IIdentityAuthenticationHandler _authenticationHandler;

        public ConfirmationRequestBinder(
            IIdentityAuthenticationHandler authenticationHandler,
            IdentityServerOptions options)
        {
            _authenticationHandler = authenticationHandler;
            _options = options;
        }

        public async Task<ConfirmationRequest> BindAsync(HttpRequest request)
        {
            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                var authenticationResult = await _authenticationHandler.AuthenticateAsync(request.HttpContext).ConfigureAwait(false);
                ClaimsIdentity identity = null;
                string code = null;
                bool rememberMe = false;

                if (authenticationResult.Succeeded)
                {
                    identity = (ClaimsIdentity)authenticationResult.Principal.Identity;
                    rememberMe = authenticationResult.Properties.IsPersistent;
                }
                else
                {
                    throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse());
                }

                if (request.Path.StartsWithSegments($"{_options.AccountEndpoint}/confirm", out var remaining))
                {
                    code = remaining.Value.TrimStart('/');
                }

                return new ConfirmationRequest(identity, code, rememberMe);
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }
        }
    }
}
