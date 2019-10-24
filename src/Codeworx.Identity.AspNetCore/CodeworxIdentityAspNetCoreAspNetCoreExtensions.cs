using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.Cryptography.Json;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Token;
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
        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IConfiguration configuration)
        {
            return AddCodeworxIdentity(
                collection,
                configuration.GetSection("Identity"),
                configuration.GetSection("AuthorizationCode"));
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IConfigurationSection identitySection, IConfigurationSection authCodeSection)
        {
            collection.Configure<IdentityOptions>(identitySection);
            collection.Configure<AuthorizationCodeOptions>(authCodeSection);

            var options = new IdentityOptions();
            identitySection.Bind(options);

            return AddCodeworxIdentity(
                collection,
                options);
        }

        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IdentityOptions identityOptions, AuthorizationCodeOptions authCodeOptions)
        {
            collection.AddOptions();
            collection.AddSingleton<IConfigureOptions<IdentityOptions>>(sp => new ConfigureOptions<IdentityOptions>(identityOptions.CopyTo));
            collection.AddSingleton<IConfigureOptions<AuthorizationCodeOptions>>(sp => new ConfigureOptions<AuthorizationCodeOptions>(authCodeOptions.CopyTo));

            return AddCodeworxIdentity(
                collection,
                identityOptions);
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
                       p => p.Request.Path.Equals(options.OauthEndpoint + "/token"),
                       p => p.UseMiddleware<TokenMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.OauthEndpoint),
                       p => p
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UseMiddleware<AuthorizationMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/login"),
                       p => p.UseMiddleware<LoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/logout"),
                       p => p.UseMiddleware<LogoutMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/me"),
                       p => p
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UseMiddleware<ProfileMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/winlogin"),
                       p => p.UseMiddleware<WindowsLoginMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/providers"),
                       p => p.UseMiddleware<ProvidersMiddleware>())
                   .MapWhen(
                       p => p.Request.Path.Equals(options.AccountEndpoint + "/tenants"),
                       p => p.UseMiddleware<TenantsMiddleware>())
                   .MapWhen(
                       EmbeddedResourceMiddleware.Condition,
                       p => p.UseMiddleware<EmbeddedResourceMiddleware>());
        }

        private static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, IdentityOptions identityOptions)
        {
            var builder = new IdentityServiceBuilder(collection);
            builder.AddPart(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            builder.View<DefaultViewTemplate>();
            builder.Pbkdf2();

            collection.AddTransient<IContentTypeProvider, DefaultContentTypeProvider>();
            collection.AddSingleton(p => builder.ToService(p.GetService<IOptions<IdentityOptions>>().Value, p.GetService<IEnumerable<IContentTypeProvider>>()));

            collection.AddAuthentication(authOptions => { authOptions.DefaultScheme = identityOptions.AuthenticationScheme; })
                      .AddCookie(
                                 identityOptions.AuthenticationScheme,
                                 p =>
                                 {
                                     p.Cookie.Name = identityOptions.AuthenticationCookie;
                                     p.LoginPath = "/account/login";
                                     p.ExpireTimeSpan = identityOptions.CookieExpiration;
                                 })
                      .AddCookie(
                                 identityOptions.MissingTenantAuthenticationScheme,
                                 p =>
                                 {
                                     var options = new IdentityOptions();
                                     builder.OptionsDelegate(options);

                                     p.Cookie.Name = identityOptions.MissingTenantAuthenticationCookie;
                                     p.LoginPath = "/account/login";
                                     p.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                                 });

            collection.AddDistributedMemoryCache();

            collection.AddTransient<IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse>, AuthorizationRequestBinder>();
            collection.AddTransient<IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse>, AuthorizationCodeTokenRequestBinder>();
            collection.AddTransient<IResponseBinder, AuthorizationErrorResponseBinder>();
            collection.AddTransient<IResponseBinder, AuthorizationCodeResponseBinder>();
            collection.AddTransient<IResponseBinder, AuthorizationTokenResponseBinder>();
            collection.AddTransient<IResponseBinder, TokenErrorResponseBinder>();
            collection.AddTransient<IResponseBinder, TokenResponseBinder>();
            collection.AddTransient<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>, AuthorizationRequestValidator>();
            collection.AddTransient<IRequestValidator<TokenRequest, TokenErrorResponse>, TokenRequestValidator>();
            collection.AddTransient<IAuthorizationCodeGenerator, AuthorizationCodeGenerator>();
            collection.AddTransient<IClientAuthenticationService, ClientAuthenticationService>();
            collection.AddSingleton<IDefaultSigningKeyProvider, DefaultSigningKeyProvider>();
            collection.AddSingleton<ITokenProvider, JwtProvider>();
            collection.AddSingleton<IAuthorizationCodeCache, DistributedAuthorizationCodeCache>();

            collection.AddScoped<IAuthorizationFlowService, AuthorizationCodeFlowService>();
            collection.AddScoped<IAuthorizationFlowService, AuthorizationTokenFlowService>();
            collection.AddScoped<IAuthorizationService, AuthorizationService>();
            collection.AddScoped<ITokenFlowService, AuthorizationCodeTokenFlowService>();
            collection.AddScoped<ITokenService, TokenService>();

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