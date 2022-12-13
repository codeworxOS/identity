using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView.Mfa
{
    public class MfaProviderListRequestBinder : IRequestBinder<MfaProviderListRequest>
    {
        private readonly IdentityOptions _options;
        private readonly IIdentityAuthenticationHandler _handler;
        private readonly ILoginService _loginService;
        private readonly IServiceProvider _serviceProvider;

        public MfaProviderListRequestBinder(IIdentityAuthenticationHandler handler, ILoginService loginService, IOptionsSnapshot<IdentityOptions> options, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _handler = handler;
            _loginService = loginService;
            _serviceProvider = serviceProvider;
        }

        public async Task<MfaProviderListRequest> BindAsync(HttpRequest request)
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

            var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;

            if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                return new MfaProviderListRequest(identity, returnUrl);
            }
            else
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("this should not happen!"));
        }
    }
}
