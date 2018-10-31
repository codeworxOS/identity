using System;
using System.Collections.Generic;
using System.Text;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using Codeworx.Identity.ContentType;
using System.Collections;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.Mvc
{
    public static class CodeworxIdentityMvcExtensions
    {
        public static IdentityServiceBuilder AddCodeworxIdentity(this IServiceCollection collection, string authenticationScheme = null)
        {
            authenticationScheme = authenticationScheme ?? Constants.DefaultAuthenticationScheme;

            var builder = new IdentityServiceBuilder(collection, authenticationScheme);
            builder.AddPart(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            builder.View<DefaultViewTemplate>();

            collection.AddTransient<IContentTypeProvider, DefaultContentTypeProvider>();
            collection.AddSingleton(p => builder.ToService(p.GetService<IOptions<IdentityOptions>>().Value, p.GetService<IEnumerable<IContentTypeProvider>>()));
            collection.AddOptions();
            collection.AddSingleton<IConfigureOptions<IdentityOptions>>(sp => new ConfigureOptions<IdentityOptions>(builder.OptionsDelegate));

            collection.AddAuthentication(authOptions =>
            {
                authOptions.DefaultScheme = authenticationScheme;
            })
                .AddCookie(authenticationScheme, p =>
                {
                    var options = new IdentityOptions();
                    builder.OptionsDelegate(options);

                    p.Cookie.Name = options.AuthenticationCookie;
                    p.LoginPath = "/account/login";
                    p.ExpireTimeSpan = options.CookieExpiration;
                });

            return builder;
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
                        p => p.UseMiddleware<OAuthTokenMiddleware>())
                    .MapWhen(
                        p => p.Request.Path.Equals(options.OauthEndpoint),
                        p => p.UseMiddleware<OAuthAuthorizationMiddleware>())
                    .MapWhen(
                        p => p.Request.Path.Equals("/account/login"),
                        p => p.UseMiddleware<LoginMiddleware>())
                    .MapWhen(
                        p => p.Request.Path.Equals("/account/me"),
                        p => p.UseMiddleware<ProfileEnpointMiddleware>())
                    .MapWhen(
                        p => p.Request.Path.Equals("/account/winlogin"),
                        p => p.UseMiddleware<WindowsLoginMiddleware>())
                    .MapWhen(
                        p => p.Request.Path.Equals("/account/providers"),
                        p => p.UseMiddleware<ProvidersMiddleware>())
                    .MapWhen(
                        EmbeddedResourceMiddleware.Condition,
                        p => p.UseMiddleware<EmbeddedResourceMiddleware>());
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