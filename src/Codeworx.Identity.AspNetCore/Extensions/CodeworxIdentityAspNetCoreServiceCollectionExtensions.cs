using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Codeworx.Identity;
using Codeworx.Identity.Account;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.AspNetCore.Binder;
using Codeworx.Identity.AspNetCore.Binder.Account;
using Codeworx.Identity.AspNetCore.Binder.Invitation;
using Codeworx.Identity.AspNetCore.Binder.Login;
using Codeworx.Identity.AspNetCore.Binder.Login.OAuth;
using Codeworx.Identity.AspNetCore.Binder.LoginView;
using Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa;
using Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa.Mail;
using Codeworx.Identity.AspNetCore.Binder.Logout;
using Codeworx.Identity.AspNetCore.Binder.SelectTenantView;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.AspNetCore.Login.Binder;
using Codeworx.Identity.AspNetCore.OAuth.Binder;
using Codeworx.Identity.AspNetCore.OpenId.Binder;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.Cryptography.Json;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Mfa.Mail;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.OpenId;
using Codeworx.Identity.OpenId.Model;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;
using Codeworx.Identity.Token;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityAspNetCoreServiceCollectionExtensions
    {
        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection)
        {
            collection.PostConfigure<IdentityOptions>(p => { });
            collection.PostConfigure<AuthorizationCodeOptions>(p => { });

            return AddCodeworxIdentity(
                collection,
                p => { },
                p => { });
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, Action<IdentityServerOptions> identitySchemaOptions)
        {
            collection.PostConfigure<IdentityOptions>(p => { });
            collection.PostConfigure<AuthorizationCodeOptions>(p => { });

            return AddCodeworxIdentity(
                collection,
                identitySchemaOptions,
                p => { });
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, Action<IdentityServerOptions> identitySchemaOptions, Action<CookieAuthenticationOptions> cookieOptions)
        {
            collection.AddOptions();
            collection.PostConfigure<IdentityOptions>(p => { });
            collection.PostConfigure<AuthorizationCodeOptions>(p => { });
            var options = new IdentityServerOptions();
            identitySchemaOptions(options);
            collection.AddSingleton(options);

            return AddCodeworxIdentity(
                collection,
                options,
                cookieOptions);
        }

        private static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IdentityServerOptions identityServerOptions, Action<CookieAuthenticationOptions> cookieOptions)
        {
            var builder = new IdentityServiceBuilder(collection);
            builder.Argon2()
                .WithAesSymmetricEncryption();

            collection.AddAuthentication(authOptions => { authOptions.DefaultScheme = identityServerOptions.AuthenticationScheme; })
                      .AddCookie(
                                 identityServerOptions.AuthenticationScheme,
                                 p =>
                                 {
                                     p.Events.OnValidatePrincipal = OnValidatePrincipal;
                                     p.SlidingExpiration = true;
                                     p.Cookie.Name = identityServerOptions.AuthenticationCookie;
                                     p.LoginPath = identityServerOptions.AccountEndpoint + "/login";
                                     p.ExpireTimeSpan = identityServerOptions.CookieExpiration;
                                     cookieOptions?.Invoke(p);
                                 })
                    .AddCookie(
                                 identityServerOptions.MfaAuthenticationScheme,
                                 p =>
                                 {
                                     p.Events.OnValidatePrincipal = OnValidatePrincipal;
                                     p.SlidingExpiration = true;
                                     p.Cookie.Name = identityServerOptions.MfaAuthenticationCookie;
                                     p.LoginPath = identityServerOptions.AccountEndpoint + "/login/mfa";
                                     p.ExpireTimeSpan = identityServerOptions.CookieExpiration;
                                     cookieOptions?.Invoke(p);
                                 });

            collection.AddDistributedMemoryCache();
            collection.AddHttpContextAccessor();

            // Request binder
            collection.AddTransient<IRequestBinder<WindowsLoginRequest>, WindowsLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<OAuthLoginRequest>, OAuthLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<Codeworx.Identity.OAuth.AuthorizationRequest>, Codeworx.Identity.AspNetCore.OAuth.Binder.AuthorizationRequestBinder>();
            collection.AddTransient<IRequestBinder<Codeworx.Identity.OpenId.AuthorizationRequest>, Codeworx.Identity.AspNetCore.OpenId.Binder.AuthorizationRequestBinder>();
            collection.AddTransient<IRequestBinder<ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<TokenRequest>, TokenRequestBinder>();
            collection.AddTransient<IRequestBinder<RefreshTokenRequest>, RefreshTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<TokenExchangeRequest>, TokenExchangeRequestBinder>();
            collection.AddTransient<IRequestBinder<LoginRequest>, LoginRequestBinder>();
            collection.AddTransient<IRequestBinder<LogoutRequest>, LogoutRequestBinder>();
            collection.AddTransient<IRequestBinder<MfaLoginRequest>, MfaLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<MfaProviderListRequest>, MfaProviderListRequestBinder>();
            collection.AddTransient<IRequestBinder<SelectTenantViewRequest>, SelectTenantViewRequestBinder>();
            collection.AddTransient<IRequestBinder<SelectTenantViewActionRequest>, SelectTenantViewActionRequestBinder>();
            collection.AddTransient<IRequestBinder<OAuthRedirectRequest>, OAuthRedirectRequestBinder>();
            collection.AddTransient<IRequestBinder<ExternalCallbackRequest>, ExternalCallbackRequestBinder>();
            collection.AddTransient<IRequestBinder<RedirectRequest>, RedirectRequestBinder>();
            collection.AddTransient<IRequestBinder<InvitationViewRequest>, InvitationViewRequestBinder>();
            collection.AddTransient<IRequestBinder<PasswordChangeRequest>, PasswordChangeRequestBinder>();
            collection.AddTransient<IRequestBinder<ProfileRequest>, ProfileRequestBinder>();
            collection.AddTransient<IRequestBinder<ProfileLinkRequest>, ProfileLinkRequestBinder>();
            collection.AddTransient<IRequestBinder<ForgotPasswordRequest>, ForgotPasswordRequestBinder>();
            collection.AddTransient<IRequestBinder<ConfirmationRequest>, ConfirmationRequestBinder>();
            collection.AddTransient<IRequestBinder<MailLoginRequest>, MailLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<IntrospectRequest>, IntrospectRequestBinder>();

            // Response binder
            collection.AddTransient<IResponseBinder<WindowsChallengeResponse>, WindowsChallengeResponseBinder>();
            collection.AddTransient<IResponseBinder<NotAcceptableResponse>, NotAcceptableResponseBinder>();
            collection.AddTransient<IResponseBinder<UnauthorizedResponse>, UnauthorizedResponseBinder>();
            collection.AddTransient<IResponseBinder<ForbiddenResponse>, ForbiddenResponseBinider>();
            collection.AddTransient<IResponseBinder<AuthorizationErrorResponse>, AuthorizationErrorResponseBinder>();
            collection.AddTransient<IResponseBinder<AuthorizationSuccessResponse>, AuthorizationSuccessResponseBinder>();
            collection.AddTransient<IResponseBinder<ErrorResponse>, ErrorResponseBinder>();
            collection.AddTransient<IResponseBinder<TokenResponse>, TokenResponseBinder>();
            collection.AddTransient<IResponseBinder<SignInResponse>, SignInResponseBinder>();
            collection.AddTransient<IResponseBinder<AssetResponse>, AssetResponseBinder>();
            collection.AddTransient<IResponseBinder<RegistrationInfoResponse>, ProviderInfosResponseBinder>();
            collection.AddTransient<IResponseBinder<MethodNotSupportedResponse>, MethodNotSupportedResponseBinder>();
            collection.AddTransient<IResponseBinder<UnsupportedMediaTypeResponse>, UnsupportedMediaTypeResponseBinder>();
            collection.AddTransient<IResponseBinder<LoginResponse>, LoginResponseBinder>();
            collection.AddTransient<IResponseBinder<MfaLoginResponse>, MfaLoginResponseBinder>();
            collection.AddTransient<IResponseBinder<MfaProviderListResponse>, MfaProviderListResponseBinder>();
            collection.AddTransient<IResponseBinder<ProfileResponse>, ProfileResponseBinder>();
            collection.AddTransient<IResponseBinder<InvalidStateResponse>, InvalidStateResponseBinder>();
            collection.AddTransient<IResponseBinder<WellKnownResponse>, WellKnownResponseBinder>();
            collection.AddTransient<IResponseBinder<UserInfoResponse>, UserInfoResponseBinder>();
            collection.AddTransient<IResponseBinder<SelectTenantViewResponse>, SelectTenantViewResponseBinder>();
            collection.AddTransient<IResponseBinder<SelectTenantSuccessResponse>, SelectTenantSuccessResponseBinder>();
            collection.AddTransient<IResponseBinder<MissingMfaResponse>, MissingMfaResponseBinder>();
            collection.AddTransient<IResponseBinder<MissingTenantResponse>, MissingTenantResponseBinder>();
            collection.AddTransient<IResponseBinder<OAuthRedirectResponse>, OAuthRedirectResponseBinder>();
            collection.AddTransient<IResponseBinder<LoginChallengeResponse>, LoginChallengeResponseBinder>();
            collection.AddTransient<IResponseBinder<LoginRedirectResponse>, LoginRedirectResponseBinder>();
            collection.AddTransient<IResponseBinder<LoggedinResponse>, LoggedinResponseBinder>();
            collection.AddTransient<IResponseBinder<LogoutResponse>, LogoutResponseBinder>();
            collection.AddTransient<IResponseBinder<RedirectViewResponse>, RedirectViewResponseBinder>();
            collection.AddTransient<IResponseBinder<InvitationViewResponse>, InvitationViewResponseBinder>();
            collection.AddTransient<IResponseBinder<PasswordChangeViewResponse>, PasswordChangeViewResponseBinder>();
            collection.AddTransient<IResponseBinder<PasswordChangeResponse>, PasswordChangeResponseBinder>();
            collection.AddTransient<IResponseBinder<ProfileResponse>, ProfileResponseBinder>();
            collection.AddTransient<IResponseBinder<ProfileLinkResponse>, ProfileLinkResponseBinder>();
            collection.AddTransient<IResponseBinder<ForceChangePasswordResponse>, ForceChangePasswordResponseBinder>();
            collection.AddTransient<IResponseBinder<ForgotPasswordViewResponse>, ForgotPasswordViewResponseBinder>();
            collection.AddTransient<IResponseBinder<ForgotPasswordResponse>, ForgotPasswordResponseBinder>();
            collection.AddTransient<IResponseBinder<ConfirmationResponse>, ConfirmationResponseBinder>();
            collection.AddTransient<IResponseBinder<IIntrospectResponse>, IntrospectResponseBinder>();

            collection.AddScoped<ITokenRequestBindingSelector, AuthorizationCodeBindingSelector>();
            collection.AddScoped<ITokenRequestBindingSelector, ClientCredentialsBindingSelector>();
            collection.AddScoped<ITokenRequestBindingSelector, RefreshTokenBindingSelector>();
            collection.AddScoped<ITokenRequestBindingSelector, TokenExchangeBindingSelector>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OAuth.AuthorizationRequest>, StageOneAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OAuth.AuthorizationRequest>, StageTwoAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, StageOneAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, StageTwoAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OAuth.AuthorizationRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OAuth.Token.ClientCredentialsTokenRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>, Codeworx.Identity.ScopeIdentityDataRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, Codeworx.Identity.OpenId.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, Codeworx.Identity.OpenId.ScopeIdentityDataRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OAuth.AuthorizationRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OAuth.Token.ClientCredentialsTokenRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>, TenantAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OAuth.AuthorizationRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Codeworx.Identity.OpenId.AuthorizationRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OAuth.Token.ClientCredentialsTokenRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OAuth.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestValidationProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestValidationProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OAuth.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestUserLookupProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Codeworx.Identity.OpenId.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestUserLookupProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestValidationProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestClientProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestUserProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestScopeProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>, TokenExchangeRequestValidationProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>, TokenExchangeRequestUserLookupProcessor>();

            collection.AddTransient<IScopeService, ScopeService>();
            collection.AddSingleton<ISystemScopeProvider, SystemScopeProvider>();
            collection.AddTransient<ISystemScopeProvider, TenantScopeProvider>();
            collection.AddTransient<ISystemScopeProvider, ExternalTokenScopeProvider>();

            collection.AddTransient<IClaimsService, ClaimsService>();
            collection.AddScoped<ISystemClaimsProvider, OAuthClaimsProvider>();
            collection.AddScoped<ISystemClaimsProvider, TenantClaimsProvider>();
            collection.AddScoped<ISystemClaimsProvider, ExternalTokenClaimsProvider>();
            collection.AddSingleton<ISystemClaimsProvider, OpenIdClaimsProvider>();

            collection.AddTransient<IIntrospectionService, IntrospectionService>();

            collection.AddSingleton<IAuthorizationCodeGenerator, AuthorizationCodeGenerator>();
            collection.AddTransient<IClientAuthenticationService, ClientAuthenticationService>();
            collection.AddSingleton<IDefaultSigningKeyProvider, DefaultSigningKeyProvider>();
            collection.AddTransient<ITokenProvider, JwtProvider>();
            collection.AddSingleton<IAuthorizationCodeCache, DistributedAuthorizationCodeCache>();
            collection.AddSingleton<ITokenCache, DistributedTokenCache>();
            collection.AddSingleton<IExternalTokenCache, DistributedExternalTokenCache>();
            collection.AddSingleton<IStateLookupCache, DistributedStateLookupCache>();
            collection.AddSingleton<IMailMfaCodeCache, DistributedMailMfaCodeCache>();
            collection.AddSingleton<ITemplateCompiler, MustacheTemplateCompiler>();

            collection.AddSingleton<IIdentityAuthenticationHandler, DefaultIdentityAuthenticationHandler>();

            collection.AddTransient<IJwkInformationSerializer, RsaJwkSerializer>();
            collection.AddTransient<IJwkInformationSerializer, EcdJwkSerializer>();

            collection.AddScoped<IBaseUriAccessor, HttpContextBaseUriAccessor>();

            collection.AddHttpClient<IExternalOAuthTokenService, ExternalOAuthTokenService>();

            collection.AddSingleton<IStringResources, DefaultStringResources>();

            return builder;
        }

        private static string GetFormKeyName(PropertyInfo item)
        {
            var dataMember = item.GetCustomAttribute<DataMemberAttribute>();
            return dataMember?.Name ?? item.Name;
        }

        private static async Task OnValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Properties.AllowRefresh ?? true)
            {
#if NET8_0_OR_GREATER
                var timeProvider = context.HttpContext.RequestServices.GetRequiredService<TimeProvider>();
                var currentUtc = timeProvider.GetUtcNow();
#else
                var clock = context.HttpContext.RequestServices.GetRequiredService<ISystemClock>();
                var currentUtc = clock.UtcNow;
#endif
                var issuedUtc = context.Properties.IssuedUtc;
                var expiresUtc = context.Properties.ExpiresUtc;
                var allowRefresh = context.Properties.AllowRefresh ?? true;
                if (issuedUtc != null && expiresUtc != null && allowRefresh)
                {
                    var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                    var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                    if (timeRemaining < timeElapsed)
                    {
                        var cache = context.HttpContext.RequestServices.GetRequiredService<IExternalTokenCache>();
                        var key = context.Principal.FindFirst(Constants.Claims.ExternalTokenKey)?.Value;
                        if (key != null)
                        {
                            var extend = expiresUtc.Value.Subtract(issuedUtc.Value);
                            await cache.ExtendAsync(key, extend).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private static async Task WriteJsonObjectAsync(JsonTextWriter writer, IFormCollection form, IEnumerable<string> formKeys, string parentPath = null)
        {
            var keys = formKeys.GroupBy(p => p.Split('_').First()).ToDictionary(
                p => p.Key,
                p => p.Select(x => string.Join("_", x.Split('_').Skip(1)))
                      .Where(y => !string.IsNullOrWhiteSpace(y))
                      .ToList());

            await writer.WriteStartObjectAsync();
            foreach (var item in keys)
            {
                var currentPath = parentPath != null ? $"{parentPath}_{item.Key}" : item.Key;
                await writer.WritePropertyNameAsync(item.Key);
                if (item.Value.Any())
                {
                    await WriteJsonObjectAsync(writer, form, item.Value, currentPath);
                }
                else
                {
                    var values = form[currentPath];

                    if (values.Count > 1)
                    {
                        await writer.WriteStartArrayAsync();
                    }

                    foreach (var value in values)
                    {
                        await writer.WriteValueAsync(value);
                    }

                    if (values.Count > 1)
                    {
                        await writer.WriteEndArrayAsync();
                    }
                }
            }

            await writer.WriteEndObjectAsync();
        }

        private static async Task WriteJsonObjectAsync(JsonTextWriter writer, IEnumerable<KeyValuePair<string, StringValues>> query)
        {
            var properties = query.GroupBy(p => p.Key, p => p.Value)
                                  .ToDictionary(g => g.Key, g => g.ToList());

            await writer.WriteStartObjectAsync();
            foreach (var item in properties)
            {
                await writer.WritePropertyNameAsync(item.Key);
                var values = item.Value;

                if (values.Count > 1)
                {
                    await writer.WriteStartArrayAsync();
                }

                foreach (var value in values)
                {
                    await writer.WriteValueAsync(value);
                }

                if (values.Count > 1)
                {
                    await writer.WriteEndArrayAsync();
                }
            }

            await writer.WriteEndObjectAsync();
        }
    }
}