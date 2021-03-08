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
                return Task.FromResult(new InvitationViewRequest(code));
            }

            throw new NotSupportedException("Invalid invitation request");
        }
    }
}
