using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class OAuthLoginRequestBinder : IRequestBinder<OAuthLoginRequest>
    {
        private readonly IAuthenticationSchemeProvider _schemaProvider;

        public OAuthLoginRequestBinder(IAuthenticationSchemeProvider schemaProvider)
        {
            _schemaProvider = schemaProvider;
        }

        public async Task<OAuthLoginRequest> BindAsync(HttpRequest request)
        {
            Dictionary<string, StringValues> parameters;

            if (request.HasFormContentType)
            {
                var formCollection = await request.ReadFormAsync();

                parameters = formCollection.ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                parameters = request.Query.ToDictionary(p => p.Key, p => p.Value);
            }

            var valid = true;
            valid &= parameters.TryGetValue("code", out StringValues codeValues);
            valid &= parameters.TryGetValue("state", out StringValues stateValues);

            if (valid)
            {
                return new OAuthLoginRequest(codeValues, stateValues);
            }

            throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
        }
    }
}