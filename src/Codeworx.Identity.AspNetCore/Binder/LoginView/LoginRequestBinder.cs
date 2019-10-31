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

            var authenticateResult = await request.HttpContext.AuthenticateAsync(_options.AuthenticationScheme);

            if (authenticateResult.Succeeded)
            {
                return new LoggedinRequest((ClaimsIdentity)authenticateResult.Principal.Identity, returnUrl);
            }

            var tenantAuthenticateResult = await request.HttpContext.AuthenticateAsync(_options.MissingTenantAuthenticationScheme);

            if (tenantAuthenticateResult.Succeeded)
            {
                var tenantIdentity = (ClaimsIdentity)tenantAuthenticateResult.Principal.Identity;

                if (HttpMethods.IsGet(request.Method))
                {
                    return new TenantMissingRequest(returnUrl, tenantIdentity);
                }
                else if (HttpMethods.IsPost(request.Method))
                {
                    if (request.HasFormContentType)
                    {
                        var tenantKey = request.Form["tenantKey"].FirstOrDefault();
                        bool setDefault = false;

                        if (request.Form.TryGetValue("setDefault", out var values))
                        {
                            if (bool.TryParse(values.First(), out var value))
                            {
                                setDefault = value;
                            }
                        }

                        return new TenantSelectionRequest(returnUrl, tenantIdentity, tenantKey, setDefault);
                    }

                    throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
                }
                else
                {
                    throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
                }
            }
            else
            {
                if (HttpMethods.IsGet(request.Method))
                {
                    return new LoginRequest(returnUrl);
                }
                else if (HttpMethods.IsPost(request.Method))
                {
                    if (request.HasFormContentType)
                    {
                        var username = request.Form["username"].FirstOrDefault();
                        var password = request.Form["password"].FirstOrDefault();

                        return new LoginFormRequest(returnUrl, username, password);
                    }

                    throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
                }
                else
                {
                    throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
                }
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("this should not happen!"));
        }
    }
}