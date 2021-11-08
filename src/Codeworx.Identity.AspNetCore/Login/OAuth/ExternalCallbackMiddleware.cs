using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class ExternalCallbackMiddleware
    {
        private readonly RequestDelegate _next;

        public ExternalCallbackMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<ExternalCallbackRequest> requestBinder,
            IResponseBinder<SignInResponse> signInBinder,
            IResponseBinder<LoginRedirectResponse> loginRedirectResponseBinder,
            ILoginService loginService,
            IStringResources stringResources)
        {
            ExternalCallbackRequest callbackRequest = null;

            try
            {
                callbackRequest = await requestBinder.BindAsync(context.Request);
                SignInResponse signInResponse = await loginService.SignInAsync(callbackRequest.ProviderId, callbackRequest.LoginRequest);
                await signInBinder.BindAsync(signInResponse, context.Response);
            }
            catch (LoginProviderNotFoundException)
            {
                var message = stringResources.GetResource(StringResource.UnknownLoginProviderError);
                var response = new LoginRedirectResponse(providerError: message);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
            catch (AuthenticationException ex)
            {
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, ex.Message);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
            catch (ErrorResponseException error)
            {
                IResponseBinder binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response);
            }
            catch (Exception)
            {
                var message = stringResources.GetResource(StringResource.GenericLoginError);
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, message);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
        }
    }
}