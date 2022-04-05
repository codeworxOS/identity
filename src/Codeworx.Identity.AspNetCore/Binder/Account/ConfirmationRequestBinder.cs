using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ConfirmationRequestBinder : IRequestBinder<ConfirmationRequest>
    {
        private readonly IdentityOptions _options;

        public ConfirmationRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public async Task<ConfirmationRequest> BindAsync(HttpRequest request)
        {
            if (HttpMethods.IsGet(request.Method))
            {
                var authenticationResult = await request.HttpContext.AuthenticateAsync();
                ClaimsIdentity identity = null;
                string code = null;

                if (authenticationResult.Succeeded)
                {
                    identity = (ClaimsIdentity)authenticationResult.Principal.Identity;
                }
                else
                {
                    throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse());
                }

                if (request.Path.StartsWithSegments($"{_options.AccountEndpoint}/confirm", out var remaining))
                {
                    code = remaining.Value.TrimStart('/');
                }

                return new ConfirmationRequest(identity, code);
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }
        }
    }
}
