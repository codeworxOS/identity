using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Account
{
    public class PasswordChangeService : IPasswordChangeService
    {
        private readonly IChangePasswordService _passwordService;
        private readonly IUserService _userService;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IPasswordPolicyProvider _policyProvider;

        public PasswordChangeService(
            IUserService userService,
            IPasswordValidator passwordValidator,
            IPasswordPolicyProvider policyProvider,
            IChangePasswordService passwordService = null)
        {
            _passwordService = passwordService;
            _userService = userService;
            _passwordValidator = passwordValidator;
            _policyProvider = policyProvider;
        }

        public async Task<PasswordChangeResponse> ProcessChangePasswordAsync(ProcessPasswordChangeRequest request)
        {
            if (_passwordService == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Password change not supported!"));
            }

            var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());

            var policy = await _policyProvider.GetPolicyAsync();
            string error = null;
            bool hasError = false;

            var isPasswordValid = await _passwordValidator.Validate(user, request.CurrentPassword);

            if (!isPasswordValid)
            {
                error = "Wrong password!";
                hasError = true;
            }
            else if (request.NewPassword != request.ConfirmPassword)
            {
                error = "Passwords do not match!";
                hasError = true;
            }
            else if (!Regex.IsMatch(request.NewPassword, policy.Regex))
            {
                error = policy.Description;
                hasError = true;
            }

            if (hasError)
            {
                var errorResponse = new PasswordChangeViewResponse(user.Name, error);
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

            var viewResponse = new PasswordChangeViewResponse(user.Name);

            return viewResponse;
        }
    }
}
