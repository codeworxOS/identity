using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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

        public async Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> AuthenticateAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync(_options.AuthenticationScheme);

            if (result.Succeeded && result.Principal.HasClaim(Constants.Claims.ForceChangePassword, "true"))
            {
                if (context.Request.Path != _options.AccountEndpoint + "/change-password")
                {
                    var returnUrl = context.Request.GetDisplayUrl();
                    throw new ErrorResponseException<ForceChangePasswordResponse>(new ForceChangePasswordResponse(returnUrl));
                }
            }

            return result;
        }

        public async Task ChallengeAsync(HttpContext context)
        {
            await context.ChallengeAsync(_options.AuthenticationScheme);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task SignInAsync(HttpContext context, ClaimsPrincipal principal, bool persist)
        {
            var properties = new AuthenticationProperties();
            if (persist)
            {
                properties.IsPersistent = true;
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90);
            }

            await context.SignInAsync(_options.AuthenticationScheme, principal, properties);
        }

        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(_options.AuthenticationScheme);
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
    }
}
