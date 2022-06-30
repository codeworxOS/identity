using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaViewService : IMfaViewService
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly IServiceProvider _serviceProvider;

        public MfaViewService(
            IUserService userService,
            ILoginService loginService,
            IServiceProvider serviceProvider)
        {
            _userService = userService;
            _loginService = loginService;
            _serviceProvider = serviceProvider;
        }

        public async Task<SignInResponse> ProcessLoginAsync(MfaProcessLoginRequest request)
        {
            var response = await _loginService.SignInAsync(request.ProviderId, request.ProviderRequestParameter).ConfigureAwait(false);

            response.Identity.AddClaim(new System.Security.Claims.Claim("mfa", request.ProviderId));
            var result = new SignInResponse(response.Identity, request.ReturnUrl);

            return result;
        }

        public async Task<MfaLoginResponse> ShowLoginAsync(MfaLoginRequest request, string errorProviderId = null, string errorMessage = null)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity);

            if (user == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("user missing!"));
            }

            var requestType = user.HasMfaRegistration ? ProviderRequestType.MfaLogin : ProviderRequestType.MfaRegister;

            var providerRequest = new ProviderRequest(requestType, request.ReturnUrl, user: user);

            if (errorProviderId != null)
            {
                providerRequest.ProviderErrors.Add(errorProviderId, errorMessage);
            }

            var response = await _loginService.GetRegistrationInfosAsync(providerRequest);

            var result = new MfaLoginResponse(response.Groups, request.ReturnUrl);

            return result;
        }
    }
}
