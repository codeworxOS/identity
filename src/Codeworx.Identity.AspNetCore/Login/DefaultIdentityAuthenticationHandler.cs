﻿using System;
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

namespace Codeworx.Identity.AspNetCore.Login
{
    public class DefaultIdentityAuthenticationHandler : IIdentityAuthenticationHandler
    {
        private IdentityServerOptions _options;

        public DefaultIdentityAuthenticationHandler(IdentityServerOptions options)
        {
            _options = options;
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
                        var baseAccessor = context.RequestServices.GetRequiredService<IBaseUriAccessor>();

                        var uriBuilder = new UriBuilder(baseAccessor.BaseUri);
                        uriBuilder.AppendPath(_options.AccountEndpoint);
                        uriBuilder.AppendPath("login");
                        uriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, Constants.OAuth.Prompt.SelectAccount);

                        if (context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out var values))
                        {
                            uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, values);
                        }

                        var userService = context.RequestServices.GetService<IUserService>();
                        var user = await userService.GetUserByIdentityAsync((ClaimsIdentity)result.Principal.Identity).ConfigureAwait(false);
                        var stringResources = context.RequestServices.GetService<IStringResources>();

                        throw new ErrorResponseException<ConfirmationResponse>(new ConfirmationResponse(user, stringResources.GetResource(StringResource.AccountConfirmationPending), uriBuilder.ToString()));
                    }
                }

                if (mode == AuthenticationMode.Login)
                {
                    var claimsIdentity = (ClaimsIdentity)result.Principal.Identity;
                    var mfaResult = await context.AuthenticateAsync(GetAuthenticationSchema(AuthenticationMode.Mfa));

                    if (mfaResult.Succeeded)
                    {
                        claimsIdentity.AddClaims(mfaResult.Principal.Claims);
                    }
                }
            }

            return result;
        }

        public async Task ChallengeAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login)
        {
            await context.ChallengeAsync(GetAuthenticationSchema(mode));
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

        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(GetAuthenticationSchema(AuthenticationMode.Mfa));
            await context.SignOutAsync(GetAuthenticationSchema(AuthenticationMode.Login));
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
