using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    // EventIds 153xx
    public partial class IntrospectMiddleware
    {
        private readonly RequestDelegate _next;

        public IntrospectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        [LoggerMessage(
                EventId = 15301,
        Level = LogLevel.Error,
        Message = "An unhandled error occured while processing an introspect request.")]
        public static partial void LogUnhandledError(ILogger logger, Exception ex);

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<IntrospectRequest> requestBinder,
            IResponseBinder<IIntrospectResponse> responseBinder,
            IIntrospectionService service,
            ILogger<IntrospectMiddleware> logger)
        {
            try
            {
                var request = await requestBinder.BindAsync(context.Request);

                var response = await service.ProcessAsync(request);

                await responseBinder.BindAsync(response, context.Response);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
            catch (Exception ex)
            {
                LogUnhandledError(logger, ex);
                await responseBinder.BindAsync(new IntrospectResponse(false), context.Response);
            }
        }
    }
}