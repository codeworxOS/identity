using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ForgotPasswordRequestBinder : IRequestBinder<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestBinder()
        {
        }

        public async Task<ForgotPasswordRequest> BindAsync(HttpRequest request)
        {
            await Task.Yield();
            string returnUrl = null, prompt = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrlValues))
            {
                returnUrl = returnUrlValues;
            }

            if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
            {
                prompt = promptValues;
            }

            if (HttpMethods.IsPost(request.Method) && request.HasFormContentType)
            {
                string login = null;

                if (request.Form.TryGetValue(Constants.Forms.UserName, out var loginValues))
                {
                    login = loginValues;
                }

                return new ProcessForgotPasswordRequest(login, returnUrl, prompt);
            }
            else if (HttpMethods.IsGet(request.Method))
            {
                return new ForgotPasswordRequest(returnUrl, prompt);
            }

            throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
        }
    }
}
