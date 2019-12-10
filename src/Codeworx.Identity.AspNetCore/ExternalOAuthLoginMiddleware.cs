using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class ExternalOAuthLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public ExternalOAuthLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<ExternalOAuthLoginRequest> requestBinder, IResponseBinder<SignInResponse> signInBinder, IExternalLoginService externalLogin)
        {
            try
            {
                ExternalOAuthLoginRequest oauthLoginRequest = await requestBinder.BindAsync(context.Request);

                SignInResponse signInResponse = await externalLogin.SignInAsync(Constants.ExternalOAuthProviderId, oauthLoginRequest);

                await signInBinder.BindAsync(signInResponse, context.Response);
            }
            catch (ErrorResponseException error)
            {
                IResponseBinder binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
        }
    }
}