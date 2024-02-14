using System;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public partial class ScimErrorAttribute : Attribute, IAsyncExceptionFilter
    {
        [LoggerMessage(Level = LogLevel.Error)]
        public static partial void LogError(ILogger logger, Exception ex);

        public Task OnExceptionAsync(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ScimErrorAttribute>>();
            LogError(logger, context.Exception);

            ErrorResource errorContent;
            int statusCode = 400;

            if (context.Exception is ScimException scimException)
            {
                statusCode = scimException.StatusCode;
                errorContent = new ErrorResource(scimException.ScimType, scimException.StatusCode);
            }
            else
            {
                errorContent = new ErrorResource(ScimType.InvalidSyntax, 400);
            }

            context.Result = new ObjectResult(errorContent) { StatusCode = statusCode };
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
