using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore
{
    public class WindowsLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public WindowsLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IRequestBinder<WindowsLoginRequest> requestBinder,
            IRequestValidator<WindowsLoginRequest> requestValidator,
            IResponseBinder<LoginRedirectResponse> loginRedirectBinder,
            IResponseBinder<SignInResponse> signInBinder,
            ILoginService loginService,
            IStringResources stringResources)
        {
            WindowsLoginRequest windowsLoginRequest = null;

            try
            {
                windowsLoginRequest = await requestBinder.BindAsync(context.Request).ConfigureAwait(false);
                await requestValidator.ValidateAsync(windowsLoginRequest).ConfigureAwait(false);
                var signInResonse = await loginService.SignInAsync(windowsLoginRequest.ProviderId, windowsLoginRequest).ConfigureAwait(false);
                await signInBinder.BindAsync(signInResonse, context.Response).ConfigureAwait(false);
            }
            catch (AuthenticationException ex)
            {
                var data = new LoginRedirectResponse(windowsLoginRequest.ProviderId, ex.Message, windowsLoginRequest.ReturnUrl);
                await loginRedirectBinder.BindAsync(data, context.Response).ConfigureAwait(false);
            }
            catch (LoginProviderNotFoundException)
            {
                var message = stringResources.GetResource(StringResource.UnknownLoginProviderError);
                var data = new LoginRedirectResponse(providerError: message, redirectUri: windowsLoginRequest.ReturnUrl);
                await loginRedirectBinder.BindAsync(data, context.Response).ConfigureAwait(false);
            }
            catch (ErrorResponseException error)
            {
                var binder = context.GetResponseBinder(error.ResponseType);
                await binder.BindAsync(error.Response, context.Response).ConfigureAwait(false);
            }
            catch (Exception)
            {
                var message = stringResources.GetResource(StringResource.GenericLoginError);
                var data = new LoginRedirectResponse(windowsLoginRequest.ProviderId, message, windowsLoginRequest.ReturnUrl);
                await loginRedirectBinder.BindAsync(data, context.Response).ConfigureAwait(false);
            }
        }
    }
}