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
            catch (Exception ex) when (IsExceptionOfType<LoginProviderNotFoundException>(ex))
            {
                var returnUrl = GetReturnUrl(ex);
                var message = stringResources.GetResource(StringResource.UnknownLoginProviderError);
                var response = new LoginRedirectResponse(providerError: message, redirectUri: returnUrl);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
            catch (Exception ex) when (IsExceptionOfType<AuthenticationException>(ex))
            {
                var exception = GetException<AuthenticationException>(ex);
                var returnUrl = GetReturnUrl(ex);
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, exception.Message, returnUrl);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
            catch (Exception ex) when (IsExceptionOfType<ErrorResponseException>(ex))
            {
                var exception = GetException<ErrorResponseException>(ex);
                IResponseBinder binder = context.GetResponseBinder(exception.ResponseType);
                await binder.BindAsync(exception.Response, context.Response);
            }
            catch (Exception ex)
            {
                var returnUrl = GetReturnUrl(ex);
                var message = stringResources.GetResource(StringResource.GenericLoginError);
                var response = new LoginRedirectResponse(callbackRequest?.ProviderId, message, returnUrl);
                await loginRedirectResponseBinder.BindAsync(response, context.Response);
            }
        }

        private TException GetException<TException>(Exception ex)
            where TException : Exception
        {
            var exception = ex is ReturnUrlException ? ex.InnerException : ex;
            return exception as TException;
        }

        private bool IsExceptionOfType<TException>(Exception ex)
            where TException : Exception
        {
            var exception = GetException<TException>(ex);
            return exception != null;
        }

        private string GetReturnUrl(Exception ex)
        {
            var returnUrl = ex is IWithReturnUrl returnUrlException ? returnUrlException.ReturnUrl : null;
            return returnUrl;
        }
    }
}