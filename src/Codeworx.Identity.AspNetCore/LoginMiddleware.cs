﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Converter;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IViewTemplate _template;
        private readonly IContentTypeLookup _contentTypeLookup;

        public LoginMiddleware(RequestDelegate next, IViewTemplate template, IContentTypeLookup lookup)
        {
            _next = next;
            _template = template;
            _contentTypeLookup = lookup;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<IdentityOptions> options)
        {
            string body = null;
            var hasReturnUrl = context.Request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrl);

            if (context.Request.Method.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase))
            {
                var authenticateResult = await context.AuthenticateAsync(options.Value.AuthenticationScheme);

                if (authenticateResult.Succeeded)
                {
                    body = await _template.GetLoggedInTemplate(returnUrl);
                }
                else
                {
                    var tenantAuthenticateResult = await context.AuthenticateAsync(options.Value.MissingTenantAuthenticationScheme);

                    if (tenantAuthenticateResult.Succeeded)
                    {
                        var canHandleDefault = context.RequestServices.GetService<IDefaultTenantService>() != null;

                        body = await _template.GetTenantSelectionTemplate(returnUrl, canHandleDefault);
                    }
                    else
                    {
                        body = await _template.GetLoginTemplate(returnUrl);
                    }
                }
            }
            else if (context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase))
            {
                var tenantAuthenticateResult = await context.AuthenticateAsync(options.Value.MissingTenantAuthenticationScheme);

                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters =
                                  {
                                      new StringToBooleanJsonConverter()
                                  }
                };

                var loginRequest = await context.Request.BindAsync<LoginRequest>(setting);
                var tenantSelectionRequest = await context.Request.BindAsync<TenantSelectionRequest>(setting);

                if (loginRequest.UserName != null)
                {
                    try
                    {
                        var identityProvider = context.RequestServices.GetService<IIdentityService>();
                        var identityData = await identityProvider.LoginAsync(loginRequest.UserName, loginRequest.Password);
                        var principal = identityData.ToClaimsPrincipal();

                        if (identityData.TenantKey != null)
                        {
                            var authProperties = new AuthenticationProperties();

                            await context.SignInAsync(options.Value.AuthenticationScheme, principal, authProperties);
                        }
                        else
                        {
                            await context.SignInAsync(options.Value.MissingTenantAuthenticationScheme, principal);

                            var redirectLocation = hasReturnUrl ? $"login?{Constants.ReturnUrlParameter}={Uri.EscapeUriString(returnUrl)}" : "login";

                            context.Response.Redirect(redirectLocation);

                            return;
                        }
                    }
                    catch (AuthenticationException)
                    {
                    }
                }
                else if (tenantSelectionRequest.TenantKey != null && tenantAuthenticateResult.Principal?.Identity is ClaimsIdentity claimsIdentity)
                {
                    try
                    {
                        var identity = claimsIdentity.ToIdentityData();
                        var principal = new IdentityData(identity.Identifier, identity.Login, identity.Tenants, identity.Claims, tenantSelectionRequest.TenantKey).ToClaimsPrincipal();

                        await context.SignInAsync(options.Value.AuthenticationScheme, principal, new AuthenticationProperties());

                        if (tenantSelectionRequest.SetDefault)
                        {
                            var defaultTenantService = context.RequestServices.GetService<IDefaultTenantService>();

                            if (defaultTenantService != null)
                            {
                                await defaultTenantService.SetDefaultTenantAsync(identity.Identifier, tenantSelectionRequest.TenantKey);
                            }
                        }
                    }
                    catch (AuthenticationException)
                    {
                    }
                }

                if (hasReturnUrl)
                {
                    context.Response.Redirect(returnUrl);
                    return;
                }

                body = await _template.GetLoggedInTemplate(returnUrl);
            }

            if (_contentTypeLookup.TryGetContentType(".html", out string contentType))
            {
                context.Response.ContentType = contentType;
            }

            await context.Response.WriteAsync(body);
        }
    }
}