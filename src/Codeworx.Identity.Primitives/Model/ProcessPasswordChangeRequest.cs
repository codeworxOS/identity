using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ProcessPasswordChangeRequest : PasswordChangeRequest
    {
        public ProcessPasswordChangeRequest(ClaimsIdentity identity, string currentPassword, string newPassword, string confirmPassword, string returnUrl = null, string prompt = null)
            : base(identity, returnUrl, prompt)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }

        public string CurrentPassword { get; }

        public string NewPassword { get; }

        public string ConfirmPassword { get; }
    }
}
