using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.AspNetCore.Account;
using Codeworx.Identity.AspNetCore.Binder;
using Codeworx.Identity.AspNetCore.Binder.Account;
using Codeworx.Identity.AspNetCore.Binder.Invitation;
using Codeworx.Identity.AspNetCore.Binder.Login;
using Codeworx.Identity.AspNetCore.Binder.Login.OAuth;
using Codeworx.Identity.AspNetCore.Binder.LoginView;
using Codeworx.Identity.AspNetCore.Binder.Logout;
using Codeworx.Identity.AspNetCore.Binder.SelectTenantView;
using Codeworx.Identity.AspNetCore.Invitation;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.AspNetCore.OAuth.Binder;
using Codeworx.Identity.AspNetCore.OpenId;
using Codeworx.Identity.AspNetCore.OpenId.Binder;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.Cryptography.Json;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.OpenId;
using Codeworx.Identity.OpenId.Model;
using Codeworx.Identity.Response;
using Codeworx.Identity.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore
{
    public static class CodeworxIdentityAspNetCoreAspNetCoreExtensions
    {
        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IConfiguration configuration, Action<CookieAuthenticationOptions> cookieOptions = null)
        {
            return AddCodeworxIdentity(
                collection,
                configuration.GetSection("Identity"),
                configuration.GetSection("AuthorizationCode"),
                cookieOptions);
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IConfigurationSection identitySection, IConfigurationSection authCodeSection, Action<CookieAuthenticationOptions> cookieOptions = null)
        {
            collection.Configure<IdentityOptions>(identitySection);
            collection.Configure<AuthorizationCodeOptions>(authCodeSection);

            var options = new IdentityOptions();
            identitySection.Bind(options);

            return AddCodeworxIdentity(
                collection,
                options,
                cookieOptions);
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IdentityOptions identityOptions, AuthorizationCodeOptions authCodeOptions, Action<CookieAuthenticationOptions> cookieOptions = null)
        {
            collection.AddOptions();
            collection.AddSingleton<IConfigureOptions<IdentityOptions>>(sp => new ConfigureOptions<IdentityOptions>(identityOptions.CopyTo));
            collection.AddSingleton<IConfigureOptions<AuthorizationCodeOptions>>(sp => new ConfigureOptions<AuthorizationCodeOptions>(authCodeOptions.CopyTo));

            return AddCodeworxIdentity(
                collection,
                identityOptions,
                cookieOptions);
        }

        public static async Task<TModel> BindAsync<TModel>(this HttpRequest request, JsonSerializerSettings settings, bool useQueryStringOnPost = false)
        {
            Stream jsonStream = null;

            try
            {
                if (useQueryStringOnPost || HttpMethods.IsGet(request.Method))
                {
                    jsonStream = new MemoryStream();
                    using (var sw = new StreamWriter(jsonStream, Encoding.UTF8, 1024, true))
                    {
                        using (var writer = new JsonTextWriter(sw))
                        {
                            await WriteJsonObjectAsync(writer, request.Query);
                        }
                    }

                    jsonStream.Seek(0, SeekOrigin.Begin);
                }
                else if (request.HasFormContentType)
                {
                    jsonStream = new MemoryStream();
                    using (var sw = new StreamWriter(jsonStream, Encoding.UTF8, 1024, true))
                    {
                        using (var writer = new JsonTextWriter(sw))
                        {
                            await WriteJsonObjectAsync(writer, request.Form, request.Form.Keys);
                        }
                    }

                    jsonStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    jsonStream = request.Body;
                }

                var ser = JsonSerializer.Create(settings);
                using (var sr = new StreamReader(jsonStream))
                {
                    using (var jsonReader = new JsonTextReader(sr))
                    {
                        return ser.Deserialize<TModel>(jsonReader);
                    }
                }
            }
            finally
            {
                jsonStream?.Dispose();
            }
        }

        public static IApplicationBuilder UseCodeworxIdentity(this IApplicationBuilder app, IdentityOptions options)
        {
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
                            .UseMiddleware<OAuth.AuthorizationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OpenIdTokenEndpoint),
                       p => p.UseMiddleware<TokenMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OpenIdAuthorizationEndpoint),
                       p => p
                           .UseMiddleware<OpenId.AuthorizationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/login"),
                       p => p.UseMiddleware<LoginMiddleware>())
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
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/redirect"),
                       p => p.UseMiddleware<RedirectMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.StartsWithSegments(options.AccountEndpoint + "/me", out var remaining) && remaining.HasValue,
                       p => p
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UseMiddleware<ProfileLinkMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/me"),
                       p => p
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UseMiddleware<ProfileMiddleware>())
                    .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/change-password"),
                       p => p
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UseMiddleware<PasswordChangeMiddleware>())
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

        private static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IdentityOptions identityOptions, Action<CookieAuthenticationOptions> cookieOptions)
        {
            var builder = new IdentityServiceBuilder(collection);
            builder.Argon2()
                .WithAesSymmetricEncryption();

            collection.AddAuthentication(authOptions => { authOptions.DefaultScheme = identityOptions.AuthenticationScheme; })
                      .AddCookie(
                                 identityOptions.AuthenticationScheme,
                                 p =>
                                 {
                                     p.Cookie.Name = identityOptions.AuthenticationCookie;
                                     p.LoginPath = identityOptions.AccountEndpoint + "/login";
                                     p.ExpireTimeSpan = identityOptions.CookieExpiration;
                                     cookieOptions?.Invoke(p);
                                 });

            collection.AddDistributedMemoryCache();
            collection.AddHttpContextAccessor();

            // Request binder
            collection.AddTransient<IRequestBinder<WindowsLoginRequest>, WindowsLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<OAuthLoginRequest>, OAuthLoginRequestBinder>();
            collection.AddTransient<IRequestBinder<Identity.OAuth.AuthorizationRequest>, OAuth.Binder.AuthorizationRequestBinder>();
            collection.AddTransient<IRequestBinder<Identity.OpenId.AuthorizationRequest>, OpenId.Binder.AuthorizationRequestBinder>();
            collection.AddTransient<IRequestBinder<ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<TokenRequest>, TokenRequestBinder>();
            collection.AddTransient<IRequestBinder<RefreshTokenRequest>, RefreshTokenRequestBinder>();
            collection.AddTransient<IRequestBinder<LoginRequest>, LoginRequestBinder>();
            collection.AddTransient<IRequestBinder<LogoutRequest>, LogoutRequestBinder>();
            collection.AddTransient<IRequestBinder<SelectTenantViewRequest>, SelectTenantViewRequestBinder>();
            collection.AddTransient<IRequestBinder<SelectTenantViewActionRequest>, SelectTenantViewActionRequestBinder>();
            collection.AddTransient<IRequestBinder<OAuthRedirectRequest>, OAuthRedirectRequestBinder>();
            collection.AddTransient<IRequestBinder<ExternalCallbackRequest>, ExternalCallbackRequestBinder>();
            collection.AddTransient<IRequestBinder<RedirectRequest>, RedirectRequestBinder>();
            collection.AddTransient<IRequestBinder<InvitationViewRequest>, InvitationViewRequestBinder>();
            collection.AddTransient<IRequestBinder<PasswordChangeRequest>, PasswordChangeRequestBinder>();
            collection.AddTransient<IRequestBinder<ProfileRequest>, ProfileRequestBinder>();
            collection.AddTransient<IRequestBinder<ProfileLinkRequest>, ProfileLinkRequestBinder>();

            // Response binder
            collection.AddTransient<IResponseBinder<WindowsChallengeResponse>, WindowsChallengeResponseBinder>();
            collection.AddTransient<IResponseBinder<NotAcceptableResponse>, NotAcceptableResponseBinder>();
            collection.AddTransient<IResponseBinder<UnauthorizedResponse>, UnauthorizedResponseBinder>();
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
            collection.AddTransient<IResponseBinder<ProfileResponse>, ProfileResponseBinder>();
            collection.AddTransient<IResponseBinder<InvalidStateResponse>, InvalidStateResponseBinder>();
            collection.AddTransient<IResponseBinder<WellKnownResponse>, WellKnownResponseBinder>();
            collection.AddTransient<IResponseBinder<UserInfoResponse>, UserInfoResponseBinder>();
            collection.AddTransient<IResponseBinder<SelectTenantViewResponse>, SelectTenantViewResponseBinder>();
            collection.AddTransient<IResponseBinder<SelectTenantSuccessResponse>, SelectTenantSuccessResponseBinder>();
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

            collection.AddScoped<ITokenRequestBindingSelector, AuthorizationCodeBindingSelector>();
            collection.AddScoped<ITokenRequestBindingSelector, ClientCredentialsBindingSelector>();
            collection.AddScoped<ITokenRequestBindingSelector, RefreshTokenBindingSelector>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OAuth.AuthorizationRequest>, StageOneAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OAuth.AuthorizationRequest>, StageTwoAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, StageOneAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, StageTwoAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OAuth.AuthorizationRequest>, Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OAuth.Token.ClientCredentialsTokenRequest>, Identity.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, Identity.ScopeIdentityDataRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, Identity.OpenId.ScopeIdentityDataRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, Identity.OpenId.ScopeIdentityDataRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OAuth.AuthorizationRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OAuth.Token.ClientCredentialsTokenRequest>, TenantAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, TenantAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OAuth.AuthorizationRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IAuthorizationParameters, Identity.OpenId.AuthorizationRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OAuth.Token.ClientCredentialsTokenRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, ExternalTokenScopeAuthorizationRequestProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OAuth.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestValidationProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestValidationProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OAuth.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestUserLookupProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IClientCredentialsParameters, Identity.OpenId.Token.ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestUserLookupProcessor>();

            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestValidationProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestClientProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestUserProcessor>();
            collection.AddTransient<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>, RefreshTokenRequestScopeProcessor>();

            collection.AddTransient<IScopeService, ScopeService>();
            collection.AddSingleton<ISystemScopeProvider, SystemScopeProvider>();
            collection.AddTransient<ISystemScopeProvider, TenantScopeProvider>();
            collection.AddTransient<ISystemScopeProvider, ExternalTokenScopeProvider>();

            collection.AddTransient<IClaimsService, ClaimsService>();
            collection.AddScoped<ISystemClaimsProvider, OAuthClaimsProvider>();
            collection.AddScoped<ISystemClaimsProvider, TenantClaimsProvider>();
            collection.AddScoped<ISystemClaimsProvider, ExternalTokenClaimsProvider>();
            collection.AddSingleton<ISystemClaimsProvider, OpenIdClaimsProvider>();

            collection.AddSingleton<IAuthorizationCodeGenerator, AuthorizationCodeGenerator>();
            collection.AddTransient<IClientAuthenticationService, ClientAuthenticationService>();
            collection.AddSingleton<IDefaultSigningKeyProvider, DefaultSigningKeyProvider>();
            collection.AddSingleton<ITokenProvider, JwtProvider>();
            collection.AddSingleton<IAuthorizationCodeCache, DistributedAuthorizationCodeCache>();
            collection.AddSingleton<IRefreshTokenCache, DistributedRefreshTokenCache>();
            collection.AddSingleton<IExternalTokenCache, DistributedExternalTokenCache>();
            collection.AddSingleton<IStateLookupCache, DistributedStateLookupCache>();
            collection.AddSingleton<ITemplateCompiler, MustacheTemplateCompiler>();

            collection.AddTransient<IJwkInformationSerializer, RsaJwkSerializer>();
            collection.AddTransient<IJwkInformationSerializer, EcdJwkSerializer>();

            collection.AddScoped<IBaseUriAccessor, HttpContextBaseUriAccessor>();

            collection.AddHttpClient<IExternalOAuthTokenService, ExternalOAuthTokenService>();

            return builder;
        }

        private static string GetFormKeyName(PropertyInfo item)
        {
            var dataMember = item.GetCustomAttribute<DataMemberAttribute>();
            return dataMember?.Name ?? item.Name;
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