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
            var response = await _loginService.GetRegistrationInfosAsync(new ProviderRequest(request.ReturnUrl, request.Prompt));
            var user = await _userService.GetUserByIdentifierAsync(request.Identity);

            return new LoggedinResponse(user, response.Groups, request.ReturnUrl);
        }

        public async Task<LoginResponse> ProcessLoginAsync(LoginRequest request)
        {
            var response = await _loginService.GetRegistrationInfosAsync(new ProviderRequest(request.ReturnUrl, request.Prompt));

            return new LoginResponse(response.Groups, request.ReturnUrl);
        }

        public async Task<SignInResponse> ProcessLoginFormAsync(LoginFormRequest request)
        {
            try
            {
                var response = await _loginService.SignInAsync(Constants.FormsLoginProviderId, request);
                return response;
            }
            catch (AuthenticationException)
            {
                var response = await _loginService.GetRegistrationInfosAsync(new ProviderRequest(request.ReturnUrl, request.Prompt));
                var loginResponse = new LoginResponse(response.Groups, request.ReturnUrl, request.UserName, Constants.InvalidCredentialsError);

                throw new ErrorResponseException<LoginResponse>(loginResponse);
            }
        }
    }
}