using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ProfileRequestBinder : IRequestBinder<ProfileRequest>
    {
        public ProfileRequestBinder()
        {
        }

        public async Task<ProfileRequest> BindAsync(HttpRequest request)
        {
            ClaimsIdentity identity = null;
            string loginProviderId = null, loginProviderError = null;

            if (HttpMethods.IsGet(request.Method))
            {
                var authenticationResult = await request.HttpContext.AuthenticateAsync();

                if (authenticationResult.Succeeded)
                {
                    identity = (ClaimsIdentity)authenticationResult.Principal.Identity;
                }
                else
                {
                    var challenge = new LoginChallengeResponse(null);
                    throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse());
                }

                if (request.Query.TryGetValue(Constants.LoginProviderIdParameter, out var loginProviderIdValues))
                {
                    loginProviderId = loginProviderIdValues;
                }

                if (request.Query.TryGetValue(Constants.LoginProviderErrorParameter, out var loginProviderErrorValues))
                {
                    loginProviderError = loginProviderErrorValues;
                }
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            return new ProfileRequest(identity, loginProviderId, loginProviderError);
        }
    }
}