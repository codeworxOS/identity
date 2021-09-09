using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ProfileLinkRequestBinder : IRequestBinder<ProfileLinkRequest>
    {
        private readonly IdentityOptions _options;

        public ProfileLinkRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public async Task<ProfileLinkRequest> BindAsync(HttpRequest request)
        {
            ClaimsIdentity identity = null;
            string providerId = null;
            LinkDirection direction = LinkDirection.Link;

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

                if (request.Path.StartsWithSegments($"{_options.AccountEndpoint}/me", out var remaining))
                {
                    providerId = remaining.Value.TrimStart('/').Split('/')[0];
                    if (remaining.StartsWithSegments($"/{providerId}", out var directionValue))
                    {
                        if (directionValue.Equals("/unlink"))
                        {
                            direction = LinkDirection.Unlink;
                        }
                        else if (directionValue.Equals("/link"))
                        {
                            direction = LinkDirection.Link;
                        }
                        else
                        {
                            throw new ErrorResponseException<InvalidStateResponse>(new InvalidStateResponse("uri wrong"));
                        }
                    }
                }
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            return new ProfileLinkRequest(identity, providerId, direction);
        }
    }
}