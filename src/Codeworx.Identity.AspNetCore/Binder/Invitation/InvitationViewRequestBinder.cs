using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Invitation
{
    public class InvitationViewRequestBinder : IRequestBinder<InvitationViewRequest>
    {
        private readonly IdentityOptions _options;

        public InvitationViewRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public Task<InvitationViewRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments(_options.AccountEndpoint + "/invitation", out var remaining) && remaining.HasValue)
            {
                var code = remaining.Value.TrimStart('/');
                if (HttpMethods.IsGet(request.Method))
                {
                    return Task.FromResult(new InvitationViewRequest(code));
                }
                else if (HttpMethods.IsPost(request.Method))
                {
                    request.Form.TryGetValue(Constants.Forms.ProviderId, out var providerIdValue);
                    request.Form.TryGetValue(Constants.Forms.Password, out var passwordValue);
                    request.Form.TryGetValue(Constants.Forms.ConfirmPassword, out var confirmPasswordValue);
                    request.Form.TryGetValue(Constants.Forms.UserName, out var userNameValue);

                    return Task.FromResult<InvitationViewRequest>(new ProcessInvitationViewRequest(code, passwordValue, confirmPasswordValue, providerIdValue, userNameValue));
                }
            }

            throw new NotSupportedException("Invalid invitation request");
        }
    }
}
