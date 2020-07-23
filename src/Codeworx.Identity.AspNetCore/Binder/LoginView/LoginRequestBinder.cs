using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoginRequestBinder : IRequestBinder<LoginRequest>
    {
        private readonly IdentityOptions _options;

        public LoginRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public async Task<LoginRequest> BindAsync(HttpRequest request)
        {
            string returnUrl = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            string prompt = null;

            if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
            {
                prompt = promptValues.FirstOrDefault();
            }

            var authenticateResult = await request.HttpContext.AuthenticateAsync(_options.AuthenticationScheme);

            if (authenticateResult.Succeeded && !prompt.Contains(Constants.OAuth.Prompt.Login))
            {
                return new LoggedinRequest((ClaimsIdentity)authenticateResult.Principal.Identity, returnUrl, prompt);
            }

            if (HttpMethods.IsGet(request.Method))
            {
                return new LoginRequest(returnUrl, prompt);
            }
            else if (HttpMethods.IsPost(request.Method))
            {
                if (request.HasFormContentType)
                {
                    var username = request.Form["username"].FirstOrDefault();
                    var password = request.Form["password"].FirstOrDefault();

                    return new LoginFormRequest(returnUrl, username, password, prompt);
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