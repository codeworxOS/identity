using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public class LoginViewService : ILoginViewService
    {
        private readonly IDefaultTenantService _defaultTenantService;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IIdentityService _identityService;
        private readonly ITenantService _tenantService;

        public LoginViewService(ITenantService tenantService, IExternalLoginService externalLoginService, IIdentityService identityService, IDefaultTenantService defaultTenantService = null)
        {
            _defaultTenantService = defaultTenantService;
            _tenantService = tenantService;
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
                var identityData = await _identityService.LoginAsync(request.UserName, request.Password);

                return new SignInResponse(identityData, request.ReturnUrl);
            }
            catch (AuthenticationException)
            {
                var response = await _externalLoginService.GetProviderInfosAsync(new ProviderRequest(request.ReturnUrl));
                var loginResponse = new LoginResponse(response.Providers, request.ReturnUrl, request.UserName, Constants.InvalidCredentialsError);

                throw new ErrorResponseException<LoginResponse>(loginResponse);
            }
        }

        public async Task<TenantMissingResponse> ProcessTenantMissingAsync(TenantMissingRequest request)
        {
            var tenants = await _tenantService.GetTenantsByIdentityAsync(request.Identity);

            return new TenantMissingResponse(tenants, _defaultTenantService != null, request.ReturnUrl);
        }

        public Task<SignInResponse> ProcessTenantSelectionAsync(TenantSelectionRequest request)
        {
            var identity = request.Identity.ToIdentityData();
            var signInIdentity = new IdentityData(identity.Identifier, identity.Login, identity.Tenants, identity.Claims, request.TenantKey);

            return Task.FromResult(new SignInResponse(signInIdentity, request.ReturnUrl));
        }
    }
}