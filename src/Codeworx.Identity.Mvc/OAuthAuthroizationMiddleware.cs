﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace Codeworx.Identity.Mvc
{
    public class OAuthAuthroizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IdentityService _service;

        public OAuthAuthroizationMiddleware(RequestDelegate next, IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            var result = await context.AuthenticateAsync(_service.AuthenticationScheme);

            if (result.Succeeded && result.Principal == null)
            {
                await context.ChallengeAsync(_service.AuthenticationScheme);
            }
            else
            {
                await context.Response.WriteAsync($"Authorization {context.User.Identity.Name}");
            }
        }
    }
}