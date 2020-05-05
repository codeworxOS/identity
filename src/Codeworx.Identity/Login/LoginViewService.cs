using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public class LoginViewService : ILoginViewService
    {
        private readonly IExternalLoginService _externalLoginService;
        private readonly IIdentityService _identityService;

        public LoginViewService(IExternalLoginService externalLoginService, IIdentityService identityService)
        {
            _externalLoginService = externalLoginService;
            _identityService = identityService;
        }

        public async Task<LoggedinResponse> ProcessLoggedinAsync(LoggedinRequest request)
        {
            var response = await _externalLoginService.GetProviderInfosAsync(new ProviderRequest(request.ReturnUrl));

            return new LoggedinResponse(response.Providers, request.ReturnUrl);
        }

        public async Task<LoginResponse> ProcessLoginAsync(LoginRequest request)
        {
            var response = await _externalLoginService.GetProviderInfosAsync(new ProviderRequest(request.ReturnUrl));

            return new LoginResponse(response.Providers, request.ReturnUrl);
        }

        public async Task<SignInResponse> ProcessLoginFormAsync(LoginFormRequest request)
        {
            try
            {
                var identity = await _identityService.LoginAsync(request.UserName, request.Password);

                return new SignInResponse(identity, request.ReturnUrl);
            }
            catch (AuthenticationException)
            {
                var response = await _externalLoginService.GetProviderInfosAsync(new ProviderRequest(request.ReturnUrl));
                var loginResponse = new LoginResponse(response.Providers, request.ReturnUrl, request.UserName, Constants.InvalidCredentialsError);

                throw new ErrorResponseException<LoginResponse>(loginResponse);
            }
        }
    }
}