using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeRequestBinder : IRequestBinder<PasswordChangeRequest>
    {
        private readonly IdentityOptions _options;

        public PasswordChangeRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public async Task<PasswordChangeRequest> BindAsync(HttpRequest request)
        {
            string returnUrl = null, prompt = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues;
            }

            if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
            {
                prompt = promptValues;
            }

            var authenticateResult = await request.HttpContext.AuthenticateAsync(_options.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;

            if (HttpMethods.IsPost(request.Method) && request.HasFormContentType)
            {
                string currentPassword = null, newPassword = null, confirmPassword = null;

                if (request.Form.TryGetValue(Constants.Forms.CurrentPassword, out var currentPasswordValues))
                {
                    currentPassword = currentPasswordValues;
                }

                if (request.Form.TryGetValue(Constants.Forms.Password, out var newPasswordValues))
                {
                    newPassword = newPasswordValues;
                }

                if (request.Form.TryGetValue(Constants.Forms.ConfirmPassword, out var confirmPasswordValues))
                {
                    confirmPassword = confirmPasswordValues;
                }

                return new ProcessPasswordChangeRequest(identity, currentPassword, newPassword, confirmPassword, returnUrl, prompt);
            }
            else if (HttpMethods.IsGet(request.Method))
            {
                return new PasswordChangeRequest(identity, returnUrl, prompt);
            }

            throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
        }
    }
}
