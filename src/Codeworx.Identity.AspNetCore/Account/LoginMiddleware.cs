using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<LoginRequest> loginRequestBinder, IRequestValidator<LoginRequest> loginRequestValidator, ILoginViewService service)
        {
            try
            {
                object response = null;
                var request = await loginRequestBinder.BindAsync(context.Request).ConfigureAwait(false);
                IResponseBinder responseBinder = null;

                await loginRequestValidator.ValidateAsync(request).ConfigureAwait(false);

                switch (request)
                {
                    case LoginFormRequest loginForm:
                        response = await service.ProcessLoginFormAsync(loginForm);
                        responseBinder = context.GetResponseBinder<SignInResponse>();
                        break;

                    case LoggedinRequest loggedin:
                        response = await service.ProcessLoggedinAsync(loggedin);
                        responseBinder = context.GetResponseBinder<LoggedinResponse>();
                        break;

                    case LoginRequest login:
                        response = await service.ProcessLoginAsync(login);
                        responseBinder = context.GetResponseBinder<LoginResponse>();
                        break;
                    default:
                        throw new NotSupportedException();
                }

                await responseBinder.BindAsync(response, context.Response);
                return;
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
                return;
            }
        }
    }
}