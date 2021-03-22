using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class WindowsLoginRequestBinder : IRequestBinder<WindowsLoginRequest>
    {
        private readonly IAuthenticationSchemeProvider _schemaProvider;
        private readonly IInvitationService _invitationService;
        private readonly IdentityOptions _identityOptions;

        public WindowsLoginRequestBinder(
            IAuthenticationSchemeProvider schemeProvider,
            IOptionsSnapshot<IdentityOptions> options,
            IInvitationService invitationService = null)
        {
            _schemaProvider = schemeProvider;
            _invitationService = invitationService;
            _identityOptions = options.Value;
        }

        public async Task<WindowsLoginRequest> BindAsync(HttpRequest request)
        {
            var schemes = await _schemaProvider.GetAllSchemesAsync();

            if (!schemes.Any(p => p.Name.Equals(Constants.WindowsAuthenticationSchema, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Windows Authentication is disabled!"));
            }

            if (request.Path.StartsWithSegments($"{_identityOptions.AccountEndpoint}/winlogin", out var remaining))
            {
                var providerId = remaining.Value.Trim('/');

                string returnUrl = null;
                string invitationCode = null;
                if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out StringValues returnUrlValue))
                {
                    returnUrl = returnUrlValue.FirstOrDefault();
                }

                if (request.Query.TryGetValue(Constants.InvitationParameter, out var invitationValues))
                {
                    if (_invitationService == null)
                    {
                        throw new NotSupportedException(Constants.InvitationNotSupported);
                    }

                    invitationCode = invitationValues;

                    var invitation = await _invitationService.GetInvitationAsync(invitationCode).ConfigureAwait(false);
                    returnUrl = invitation.RedirectUri;
                }
                else if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("ReturnUrl parameter missing"));
                }

                var result = await request.HttpContext.AuthenticateAsync(Constants.WindowsAuthenticationSchema);

                if (result.Succeeded)
                {
                    return new WindowsLoginRequest(providerId, (ClaimsIdentity)result.Principal.Identity, returnUrl, invitationCode);
                }
                else if (result.Failure == null)
                {
                    throw new ErrorResponseException<WindowsChallengeResponse>(new WindowsChallengeResponse(returnUrl));
                }
            }

            throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
        }
    }
}