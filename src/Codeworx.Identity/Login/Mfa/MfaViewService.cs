using System;
using System.Linq;
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

            return response;
        }

        public async Task<MfaLoginResponse> ShowLoginAsync(MfaLoginRequest request, string errorMessage = null)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity);

            if (user == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("user missing!"));
            }

            var requestType = user.LinkedProviders.Contains(request.ProviderId) ? ProviderRequestType.MfaLogin : ProviderRequestType.MfaRegister;

            var providerRequest = new ProviderRequest(requestType, request.ReturnUrl, user: user);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                providerRequest.ProviderErrors.Add(request.ProviderId, errorMessage);
            }

            var registration = await _loginService.GetLoginRegistrationInfoAsync(request.ProviderId).ConfigureAwait(false);
            var processor = (ILoginProcessor)_serviceProvider.GetService(registration.ProcessorType);
            var info = await processor.GetRegistrationInfoAsync(providerRequest, registration).ConfigureAwait(false);

            var result = new MfaLoginResponse(info, request.ReturnUrl);

            return result;
        }
    }
}
