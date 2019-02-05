using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Codeworx.Identity.Mvc
{
    public abstract class AuthenticatedMiddleware
    {
        protected RequestDelegate Next;

        public AuthenticatedMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            Next = next;
            Service = service;
        }

        protected Configuration.IdentityService Service { get; }

        public async Task Invoke(HttpContext context)
        {
            var result = await context.AuthenticateAsync(Service.AuthenticationScheme);

            if (result.Succeeded)
            {
                await OnInvokeAsync(context, result.Principal);
                return;
            }
            else if (result.Failure == null)
            {
                await context.ChallengeAsync(Service.AuthenticationScheme);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        protected abstract Task OnInvokeAsync(HttpContext context, IPrincipal principal);
    }
}