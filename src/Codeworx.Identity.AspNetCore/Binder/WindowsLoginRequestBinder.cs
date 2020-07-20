using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class WindowsLoginRequestBinder : IRequestBinder<WindowsLoginRequest>
    {
        private readonly IAuthenticationSchemeProvider _schemaProvider;

        public WindowsLoginRequestBinder(IAuthenticationSchemeProvider schemeProvider)
        {
            _schemaProvider = schemeProvider;
        }

        public async Task<WindowsLoginRequest> BindAsync(HttpRequest request)
        {
            var schemes = await _schemaProvider.GetAllSchemesAsync();

            if (!schemes.Any(p => p.Name.Equals(Constants.WindowsAuthenticationSchema, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Windows Authentication is disabled!"));
            }

            string returnUrl = null;
            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out StringValues returnUrlValue))
            {
                returnUrl = returnUrlValue.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("ReturnUrl parameter missing"));
            }

            var result = await request.HttpContext.AuthenticateAsync(Constants.WindowsAuthenticationSchema);

            if (result.Succeeded)
            {
                return new WindowsLoginRequest((ClaimsIdentity)result.Principal.Identity, returnUrl);
            }
            else if (result.Failure == null)
            {
                throw new ErrorResponseException<WindowsChallengeResponse>(new WindowsChallengeResponse());
            }

            throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
        }
    }
}