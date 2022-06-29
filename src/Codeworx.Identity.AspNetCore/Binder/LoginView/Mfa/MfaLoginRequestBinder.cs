using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa
{
    public class MfaLoginRequestBinder : IRequestBinder<MfaLoginRequest>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public MfaLoginRequestBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
        }

        public async Task<MfaLoginRequest> BindAsync(HttpRequest request)
        {
            string returnUrl = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            var authenticateResult = await _handler.AuthenticateAsync(request.HttpContext);

            if (!authenticateResult.Succeeded)
            {
                throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse());
            }

            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;
                return new MfaLoginRequest(identity, returnUrl);
            }
            else if (HttpMethods.IsPost(request.Method))
            {
                if (request.HasFormContentType)
                {
                    ////var username = request.Form["username"].FirstOrDefault();
                    ////var password = request.Form["password"].FirstOrDefault();
                    ////providerId = request.Form["provider-id"].FirstOrDefault();
                    ////var remember = request.Form["remember"].FirstOrDefault() == "on";

                    ////return new LoginFormRequest(providerId, returnUrl, username, password, prompt, remember);
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
