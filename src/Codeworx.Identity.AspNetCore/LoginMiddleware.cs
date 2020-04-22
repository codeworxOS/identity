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

        public async Task Invoke(HttpContext context, IRequestBinder<LoginRequest> loginRequestBinder, ILoginViewService service)
        {
            try
            {
                object response = null;
                var request = await loginRequestBinder.BindAsync(context.Request);
                IResponseBinder responseBinder = null;

                switch (request)
                {
                    ////case TenantSelectionRequest tenantSelection:
                    ////    response = await service.ProcessTenantSelectionAsync(tenantSelection);
                    ////    responseBinder = context.GetResponseBinder<SignInResponse>();
                    ////    break;

                    ////case TenantMissingRequest missingTeant:
                    ////    response = await service.ProcessTenantMissingAsync(missingTeant);
                    ////    responseBinder = context.GetResponseBinder<TenantMissingResponse>();
                    ////    break;

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