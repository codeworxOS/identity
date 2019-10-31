using System;
using Codeworx.Identity.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Http
{
    public static class CodeworxIdentityAspNetCoreHttpContextExtensions
    {
        public static IResponseBinder<TResponse> GetResponseBinder<TResponse>(this HttpContext context)
        {
            return context.RequestServices.GetService<IResponseBinder<TResponse>>();
        }

        public static IResponseBinder GetResponseBinder(this HttpContext context, Type responseType)
        {
            return (IResponseBinder)context.RequestServices.GetService(typeof(IResponseBinder<>).MakeGenericType(responseType));
        }
    }
}
