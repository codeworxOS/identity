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
    public abstract class AuthenticatedMiddleware
    {
        protected RequestDelegate Next;

        protected AuthenticatedMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            Next = next;
            Service = service;
        }

        protected Configuration.IdentityService Service { get; }

        protected async Task<ClaimsPrincipal> Authenticate(HttpContext context)
        {
            var result = await context.AuthenticateAsync(Service.AuthenticationScheme);

            if (result.Succeeded)
            {
                return result.Principal;
            }

            if (result.Failure == null)
            {
                await context.ChallengeAsync(Service.AuthenticationScheme);
                return null;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return null;
        }
    }
}