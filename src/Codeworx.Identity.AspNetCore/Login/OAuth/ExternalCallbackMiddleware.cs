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
            IStringResources stringResources,
            IServiceProvider serviceProvider)
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
                var returnUrl = await GetReturnUrl(callbackRequest, loginService, serviceProvider);
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, ex.Message, returnUrl);
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
                var returnUrl = await GetReturnUrl(callbackRequest, loginService, serviceProvider);
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, message, returnUrl);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
        }

        private async Task<string> GetReturnUrl(ExternalCallbackRequest callbackRequest, ILoginService loginService, IServiceProvider serviceProvider)
        {
            var loginRegistration = await loginService.GetLoginRegistrationInfoAsync(callbackRequest.ProviderId);
            if (loginRegistration == null)
            {
                return null;
            }

            var processor = serviceProvider.GetService(loginRegistration.ProcessorType) as ILoginProcessor;
            if (processor == null)
            {
                return null;
            }

            var returnUrl = await processor.GetReturnUrl(loginRegistration, callbackRequest);
            return returnUrl;
        }
    }
}