using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Login
{
    public class DefaultIdentityAuthenticationHandler : IIdentityAuthenticationHandler, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private IdentityOptions _options;

        public DefaultIdentityAuthenticationHandler(IOptionsMonitor<IdentityOptions> optionsMonitor)
        {
            _options = optionsMonitor.CurrentValue;
            _subscription = optionsMonitor.OnChange(p => _options = p);
        }

        public async Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> AuthenticateAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login)
        {
            var result = await context.AuthenticateAsync(GetAuthenticationSchema(mode));

            if (result.Succeeded)
            {
                if (result.Principal.HasClaim(Constants.Claims.ForceChangePassword, "true"))
                {
                    if (context.Request.Path != _options.AccountEndpoint + "/change-password")
                    {
                        var returnUrl = context.Request.GetDisplayUrl();
                        throw new ErrorResponseException<ForceChangePasswordResponse>(new ForceChangePasswordResponse(returnUrl));
                    }
                }

                if (result.Principal.HasClaim(Constants.Claims.ConfirmationPending, "true"))
                {
                    if (!context.Request.Path.StartsWithSegments(_options.AccountEndpoint + "/confirm"))
                    {
                        var userService = context.RequestServices.GetService<IUserService>();
                        var user = await userService.GetUserByIdentityAsync((ClaimsIdentity)result.Principal.Identity).ConfigureAwait(false);
                        var stringResources = context.RequestServices.GetService<IStringResources>();

                        throw new ErrorResponseException<ConfirmationResponse>(new ConfirmationResponse(user, error: stringResources.GetResource(StringResource.AccountConfirmationPending)));
                    }
                }
            }

            return result;
        }

        public async Task ChallengeAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login)
        {
            await context.ChallengeAsync(GetAuthenticationSchema(mode));
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task SignInAsync(HttpContext context, ClaimsPrincipal principal, bool persist, AuthenticationMode mode = AuthenticationMode.Login)
        {
            var properties = new AuthenticationProperties();
            if (persist)
            {
                properties.IsPersistent = true;
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90);
            }

            await context.SignInAsync(GetAuthenticationSchema(mode), principal, properties);
        }

        public async Task SignOutAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login)
        {
            await context.SignOutAsync(GetAuthenticationSchema(mode));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }

        private string GetAuthenticationSchema(AuthenticationMode mode)
        {
            if (mode == AuthenticationMode.Mfa)
            {
                return _options.MfaAuthenticationScheme;
            }

            return _options.AuthenticationScheme;
        }
    }
}
