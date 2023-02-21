using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeRequestBinder : IRequestBinder<PasswordChangeRequest>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public PasswordChangeRequestBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
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

            var authenticateResult = await _handler.AuthenticateAsync(request.HttpContext);

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
            else if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
            {
                return new PasswordChangeRequest(identity, returnUrl, prompt);
            }

            throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
        }
    }
}
