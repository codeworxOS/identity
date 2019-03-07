using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;

        public AuthenticatedMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context, AuthenticatedUserInformation authenticatedUserInformation)
        {
            var result = await context.AuthenticateAsync(_service.AuthenticationScheme);

            if (result.Succeeded)
            {
                authenticatedUserInformation.Principal = result.Principal;
                await _next(context);
            }

            if (result.Failure == null)
            {
                await context.ChallengeAsync(_service.AuthenticationScheme);
            }
        }
    }
}