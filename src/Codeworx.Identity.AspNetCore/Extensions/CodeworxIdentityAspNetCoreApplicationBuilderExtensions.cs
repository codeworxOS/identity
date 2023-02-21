using System;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.AspNetCore.Account;
using Codeworx.Identity.AspNetCore.Invitation;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.AspNetCore.OpenId;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class CodeworxIdentityAspNetCoreApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCodeworxIdentity(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IdentityServerOptions>();

            if (options == null)
            {
                throw new InvalidOperationException("Identity services missing. Register via the AddCodeworxIdentity() method to the service provider.");
            }

            return app
                   .UseAuthentication()
                   .MapWhen(
                       p => p.Request.Path.Equals($"{options.OpenIdWellKnownPrefix}/.well-known/openid-configuration"),
                       p => p.UseMiddleware<WellKnownMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.UserInfoEndpoint),
                       p => p.UseMiddleware<UserInfoMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OpenIdJsonWebKeyEndpoint),
                       p => p.UseMiddleware<JsonWebKeyMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OauthTokenEndpoint),
                       p => p.UseMiddleware<TokenMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OauthAuthorizationEndpoint),
                       p => p
                            .UseMiddleware<Codeworx.Identity.AspNetCore.OAuth.AuthorizationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OauthInstrospectionEndpoint),
                       p => p
                            .UseMiddleware<Codeworx.Identity.AspNetCore.OAuth.IntrospectMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OpenIdTokenEndpoint),
                       p => p.UseMiddleware<TokenMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OpenIdAuthorizationEndpoint),
                       p => p
                           .UseMiddleware<Codeworx.Identity.AspNetCore.OpenId.AuthorizationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/login"),
                       p => p.UseMiddleware<LoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/login/mfa"),
                       p => p
                            .UseMiddleware<MfaAuthenticationMiddleware>()
                            .UseMiddleware<MfaProviderListMiddleware>())
                    .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/login/mfa"),
                       p => p
                            .UseMiddleware<MfaAuthenticationMiddleware>()
                            .UseMiddleware<MfaLoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/logout"),
                       p => p.UseMiddleware<LogoutMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/oauth", out var remaining) && remaining.HasValue,
                       p => p.UseMiddleware<OAuthLoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/callback", out var remaining) && remaining.HasValue,
                       p => p.UseMiddleware<ExternalCallbackMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/invitation", out var remaining) && remaining.HasValue,
                       p => p.UseMiddleware<InvitationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/confirm", out var remaining) && remaining.HasValue,
                       p => p.UseMiddleware<ConfirmationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/redirect"),
                       p => p.UseMiddleware<RedirectMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/me", out var remaining) && remaining.HasValue,
                       p => p
                            .UseMiddleware<Codeworx.Identity.AspNetCore.AuthenticationMiddleware>()
                            .UseMiddleware<ProfileLinkMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/me"),
                       p => p
                            .UseMiddleware<Codeworx.Identity.AspNetCore.AuthenticationMiddleware>()
                            .UseMiddleware<ProfileMiddleware>())
                    .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/change-password"),
                       p => p
                            .UseMiddleware<Codeworx.Identity.AspNetCore.AuthenticationMiddleware>()
                            .UseMiddleware<PasswordChangeMiddleware>())
                    .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/forgot-password"),
                       p => p
                            .UseMiddleware<ForgotPasswordMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/winlogin", out var remaining) && remaining.HasValue,
                       p => p.UseMiddleware<WindowsLoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/oauthlogin"),
                       p => p.UseMiddleware<OAuthLoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.SelectTenantEndpoint),
                       p => p.UseMiddleware<TenantsMiddleware>())
                   .MapWhen(
                       EmbeddedResourceMiddleware.Condition,
                       p => p.UseMiddleware<EmbeddedResourceMiddleware>());
        }
    }
}