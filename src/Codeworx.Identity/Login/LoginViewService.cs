using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login
{
    public class LoginViewService : ILoginViewService
    {
        private readonly ILoginService _loginService;
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IdentityOptions _options;

        public LoginViewService(ILoginService loginService, IIdentityService identityService, IUserService userService, IBaseUriAccessor baseUriAccessor, IOptions<IdentityOptions> options)
        {
            _loginService = loginService;
            _identityService = identityService;
            _userService = userService;
            _baseUriAccessor = baseUriAccessor;
            _options = options.Value;
        }

        public Task<LoggedinResponse> ProcessLoggedinAsync(LoggedinRequest loggedin)
        {
            string returnUrl = loggedin.ReturnUrl;
            UriBuilder builder = null;

            if (returnUrl == null)
            {
                builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                builder.AppendPath(_options.AccountEndpoint);
                builder.AppendPath("me");
            }
            else
            {
                builder = new UriBuilder(returnUrl);
            }

            if (loggedin.LoginProviderId != null)
            {
                builder.AppendQueryParameter(Constants.LoginProviderIdParameter, loggedin.LoginProviderId);
            }

            if (loggedin.LoginProviderError != null)
            {
                builder.AppendQueryParameter(Constants.LoginProviderErrorParameter, loggedin.LoginProviderError);
            }

            return Task.FromResult(new LoggedinResponse(builder.ToString()));
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