using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class OAuthLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public OAuthLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestBinder<OAuthLoginRequest> requestBinder, IResponseBinder<SignInResponse> signInBinder, IExternalLoginService externalLogin)
        {
            try
            {
                OAuthLoginRequest oauthLoginRequest = await requestBinder.BindAsync(context.Request);

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