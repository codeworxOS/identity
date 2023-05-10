using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginRequestBinder : IRequestBinder<LoginRequest>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public LoginRequestBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
        }

        public async Task<LoginRequest> BindAsync(HttpRequest request)
        {
            string returnUrl = null;
            string providerId = null;
            string providerError = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            if (request.Query.TryGetValue(Constants.LoginProviderIdParameter, out var providerIdValues))
            {
                providerId = providerIdValues.FirstOrDefault();
            }

            if (request.Query.TryGetValue(Constants.LoginProviderErrorParameter, out var errorValues))
            {
                providerError = errorValues.FirstOrDefault();
            }

            string prompt = null;

            if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
            {
                prompt = promptValues.FirstOrDefault();
            }

            var authenticateResult = await _handler.AuthenticateAsync(request.HttpContext);

            if (authenticateResult.Succeeded && string.IsNullOrWhiteSpace(prompt))
            {
                return new LoggedinRequest((ClaimsIdentity)authenticateResult.Principal.Identity, returnUrl, false, providerId, providerError);
            }

            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                return new LoginRequest(returnUrl, prompt, false, providerId, providerError);
            }
            else if (HttpMethods.IsPost(request.Method))
            {
                if (request.HasFormContentType)
                {
                    var username = request.Form["username"].FirstOrDefault();
                    var password = request.Form["password"].FirstOrDefault();
                    providerId = request.Form["provider-id"].FirstOrDefault();
                    var remember = request.Form["remember"].FirstOrDefault() == "on";

                    return new LoginFormRequest(providerId, returnUrl, username, password, prompt, remember, false);
                }

                throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("this should not happen!"));
        }
    }
}