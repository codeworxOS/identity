using System;
using System.Collections.Generic;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.View;
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

        public static IDictionary<string, object> GetViewContextData(this HttpResponse response, IViewData data)
        {
            var target = new Dictionary<string, object>();
            var basePath = response.HttpContext.Request.PathBase.ToString();

            target.Add("base-href", $"{basePath}/");
            data.CopyTo(target);

            return target;
        }
    }
}
