using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Binder.Login;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa
{
    public class MfaLoginRequestBinder : IRequestBinder<MfaLoginRequest>
    {
        private readonly IdentityServerOptions _options;
        private readonly IIdentityAuthenticationHandler _handler;
        private readonly ILoginService _loginService;
        private readonly IServiceProvider _serviceProvider;

        public MfaLoginRequestBinder(IIdentityAuthenticationHandler handler, ILoginService loginService, IdentityServerOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _handler = handler;
            _loginService = loginService;
            _serviceProvider = serviceProvider;
        }

        public async Task<MfaLoginRequest> BindAsync(HttpRequest request)
        {
            string returnUrl = null;
            string providerId = null;
            bool noNav = false;

            PathString basePath = $"{_options.AccountEndpoint}/login/mfa";

            if (request.Path.StartsWithSegments(basePath, out var remaining))
            {
                providerId = remaining.Value.Trim('/');
            }

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            if (request.Query.TryGetValue(Constants.NoNavParameter, out var noNavValues))
            {
                if (bool.TryParse(noNavValues, out var noNavParsed))
                {
                    noNav = noNavParsed;
                }
            }

            var authenticateResult = await _handler.AuthenticateAsync(request.HttpContext);

            if (!authenticateResult.Succeeded)
            {
                throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse());
            }

            var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;

            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                return new MfaLoginRequest(identity, HttpMethods.IsHead(request.Method), providerId, returnUrl, noNav);
            }
            else if (HttpMethods.IsPost(request.Method))
            {
                if (request.HasFormContentType)
                {
                    if (request.Form.TryGetValue("provider-id", out var providerIdValue) && providerId == providerIdValue)
                    {
                        providerId = providerIdValue;
                    }
                    else
                    {
                        throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("provider-id parameter missing"));
                    }

                    var parameterType = await _loginService.GetParameterTypeAsync(providerId).ConfigureAwait(false);
                    var factory = LoginParameterTypeFactory.GetFactory(parameterType);

                    var parameter = await factory(_serviceProvider, request).ConfigureAwait(false);
                    return new MfaProcessLoginRequest(providerId, parameter, identity, false, returnUrl, noNav);
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
