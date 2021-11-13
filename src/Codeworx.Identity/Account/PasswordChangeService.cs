using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Account
{
    public class PasswordChangeService : IPasswordChangeService
    {
        private readonly IChangePasswordService _passwordService;
        private readonly IUserService _userService;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IPasswordPolicyProvider _policyProvider;
        private readonly IStringResources _stringResources;

        public PasswordChangeService(
            IUserService userService,
            IPasswordValidator passwordValidator,
            IPasswordPolicyProvider policyProvider,
            IStringResources stringResources,
            IChangePasswordService passwordService = null)
        {
            _passwordService = passwordService;
            _userService = userService;
            _passwordValidator = passwordValidator;
            _policyProvider = policyProvider;
            _stringResources = stringResources;
        }

        public async Task<PasswordChangeResponse> ProcessChangePasswordAsync(ProcessPasswordChangeRequest request)
        {
            if (_passwordService == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Password change not supported!"));
            }

            var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());

            string error = null;
            bool hasError = false;

            var hasCurrentPassword = !string.IsNullOrEmpty(user.PasswordHash);
            if (hasCurrentPassword)
            {
                var isPasswordValid = await _passwordValidator.Validate(user, request.CurrentPassword);

                if (!isPasswordValid)
                {
                    error = _stringResources.GetResource(StringResource.PasswordChangeWrongPasswordError);
                    hasError = true;
                }
                else if (request.NewPassword == request.CurrentPassword)
                {
                    error = _stringResources.GetResource(StringResource.PasswordChangeSamePasswordError);
                    hasError = true;
                }
            }

            if (!hasError)
            {
                var policy = await _policyProvider.GetPolicyAsync();

                if (request.NewPassword != request.ConfirmPassword)
                {
                    error = _stringResources.GetResource(StringResource.PasswordChangeNotMatchingError);
                    hasError = true;
                }
                else if (user.Identity == request.NewPassword)
                {
                    error = _stringResources.GetResource(StringResource.PasswordChangeEqualToLoginError);
                    hasError = true;
                }
                else if (!Regex.IsMatch(request.NewPassword, policy.Regex))
                {
                    error = policy.GetDescription(_stringResources);
                    hasError = true;
                }
            }

            if (hasError)
            {
                var errorResponse = new PasswordChangeViewResponse(user.Name, hasCurrentPassword, error);
                throw new ErrorResponseException<PasswordChangeViewResponse>(errorResponse);
            }

            await _passwordService.SetPasswordAsync(user, request.NewPassword);

            return new PasswordChangeResponse(request.ReturnUrl, request.Prompt);
        }

        public async Task<PasswordChangeViewResponse> ShowChangePasswordViewAsync(PasswordChangeRequest request)
        {
            if (_passwordService == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Password change not supported!"));
            }

            var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());

            var hasCurrentPassword = !string.IsNullOrEmpty(user.PasswordHash);
            var viewResponse = new PasswordChangeViewResponse(user.Name, hasCurrentPassword);

            return viewResponse;
        }
    }
}
