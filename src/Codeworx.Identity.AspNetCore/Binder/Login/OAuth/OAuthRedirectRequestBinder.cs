using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Login.OAuth
{
    public class OAuthRedirectRequestBinder : IRequestBinder<OAuthRedirectRequest>
    {
        private readonly IdentityOptions _identityOptions;

        public OAuthRedirectRequestBinder(IOptionsSnapshot<IdentityOptions> options)
        {
            _identityOptions = options.Value;
        }

        public Task<OAuthRedirectRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments($"{_identityOptions.AccountEndpoint}/oauth", out var remaining))
            {
                var providerId = remaining.Value.Trim('/');

                string prompt = null;

                if (providerId.Contains("/"))
                {
                    throw new NotSupportedException($"Invalid uri {request.Path}.");
                }

                if (!request.Query.TryGetValue(Constants.ReturnUrlParameter, out var values) || values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
                {
                    throw new NotSupportedException("ReturnUrl Parameter is missing from the request.");
                }

                if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
                {
                    prompt = promptValues.FirstOrDefault();
                }

                var result = new OAuthRedirectRequest(providerId, values[0], prompt);

                return Task.FromResult(result);
            }

            throw new NotSupportedException("Invalid Uri.");
        }
    }
}
