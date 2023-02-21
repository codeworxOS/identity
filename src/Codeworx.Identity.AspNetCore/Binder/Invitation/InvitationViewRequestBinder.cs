using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Invitation
{
    public class InvitationViewRequestBinder : IRequestBinder<InvitationViewRequest>
    {
        private readonly IdentityServerOptions _options;

        public InvitationViewRequestBinder(IdentityServerOptions options)
        {
            _options = options;
        }

        public Task<InvitationViewRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments(_options.AccountEndpoint + "/invitation", out var remaining) && remaining.HasValue)
            {
                var code = remaining.Value.TrimStart('/');
                if (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method))
                {
                    return Task.FromResult(new InvitationViewRequest(code, HttpMethods.IsHead(request.Method)));
                }
                else if (HttpMethods.IsPost(request.Method))
                {
                    request.Form.TryGetValue(Constants.Forms.ProviderId, out var providerIdValue);
                    request.Form.TryGetValue(Constants.Forms.Password, out var passwordValue);
                    request.Form.TryGetValue(Constants.Forms.ConfirmPassword, out var confirmPasswordValue);
                    request.Form.TryGetValue(Constants.Forms.UserName, out var userNameValue);

                    return Task.FromResult<InvitationViewRequest>(new ProcessInvitationViewRequest(code, passwordValue, confirmPasswordValue, providerIdValue, userNameValue, false));
                }
            }

            throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
        }
    }
}
