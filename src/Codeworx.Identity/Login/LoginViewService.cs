using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public class LoginViewService : ILoginViewService
    {
        private readonly ILoginService _loginService;
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;

        public LoginViewService(ILoginService loginService, IIdentityService identityService, IUserService userService)
        {
            _loginService = loginService;
            _identityService = identityService;
            _userService = userService;
        }

        public async Task<LoggedinResponse> ProcessLoggedinAsync(LoggedinRequest request)
        {
            var response = await _loginService.GetRegistrationInfosAsync(new ProviderRequest(ProviderRequestType.Profile, request.ReturnUrl, request.Prompt, null, null));
            var user = await _userService.GetUserByIdentifierAsync(request.Identity);

            return new LoggedinResponse(user, response.Groups, request.ReturnUrl);
        }

        public async Task<LoginResponse> ProcessLoginAsync(LoginRequest request)
        {
            string error = null;
            var providerRequest = new ProviderRequest(ProviderRequestType.Login, request.ReturnUrl, request.Prompt, null, null);
            if (request.LoginProviderId != null)
            {
                providerRequest.ProviderErrors.Add(request.LoginProviderId, request.LoginProviderError ?? Constants.GenericLoginError);
            }
            else if (request.LoginProviderError != null)
            {
                error = request.LoginProviderError;
            }

            var response = await _loginService.GetRegistrationInfosAsync(providerRequest);

            return new LoginResponse(response.Groups, request.ReturnUrl, error: error);
        }

        public async Task<SignInResponse> ProcessLoginFormAsync(LoginFormRequest request)
        {
            ProviderRequest providerRequest = null;
            string errorMessage = null;

            try
            {
                var response = await _loginService.SignInAsync(request.ProviderId, request);
                return response;
            }
            catch (AuthenticationException ex)
            {
                providerRequest = new ProviderRequest(ProviderRequestType.Login, request.ReturnUrl, request.Prompt, null, null);
                providerRequest.ProviderErrors.Add(request.ProviderId, ex.Message);
            }
            catch (LoginProviderNotFoundException)
            {
                providerRequest = new ProviderRequest(ProviderRequestType.Login, request.ReturnUrl, request.Prompt, null, null);
                errorMessage = Constants.UnknownLoginProviderError;
            }
            catch (Exception)
            {
                providerRequest = new ProviderRequest(ProviderRequestType.Login, request.ReturnUrl, request.Prompt, null, null);
                providerRequest.ProviderErrors.Add(request.ProviderId, Constants.GenericLoginError);
            }

            var registrationInfos = await _loginService.GetRegistrationInfosAsync(providerRequest);
            var loginResponse = new LoginResponse(registrationInfos.Groups, request.ReturnUrl, request.UserName, errorMessage);

            throw new ErrorResponseException<LoginResponse>(loginResponse);
        }
    }
}