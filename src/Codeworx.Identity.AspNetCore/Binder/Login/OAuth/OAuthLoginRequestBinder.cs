using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.AspNetCore.Binder.Login.OAuth
{
    public class OAuthLoginRequestBinder : IRequestBinder<OAuthLoginRequest>
    {
        public OAuthLoginRequestBinder()
        {
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

            var codeAvailable = parameters.TryGetValue("code", out StringValues codeValues);

            if (codeAvailable == false)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Code parameter missing"));
            }

            string state = null;

            if (parameters.TryGetValue("state", out StringValues stateValues))
            {
                state = stateValues.First();
            }

            return new OAuthLoginRequest(codeValues, state);
        }
    }
}