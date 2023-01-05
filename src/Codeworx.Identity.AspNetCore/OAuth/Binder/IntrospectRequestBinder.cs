using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class IntrospectRequestBinder : IRequestBinder<IntrospectRequest>
    {
        public IntrospectRequestBinder()
        {
        }

        public Task<IntrospectRequest> BindAsync(HttpRequest request)
        {
            if (!HttpMethods.IsPost(request.Method))
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            if (!request.HasFormContentType)
            {
                throw new ErrorResponseException<UnsupportedMediaTypeResponse>(new UnsupportedMediaTypeResponse());
            }

            if (!request.Form.TryGetValue(Constants.OAuth.TokenName, out var token))
            {
                throw new ErrorResponseException<IIntrospectResponse>(new IntrospectResponse(false));
            }

            if (AuthenticationHeaderValue.TryParse(request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
            {
                if (authenticationHeaderValue.Scheme.Equals(Constants.BasicHeader, System.StringComparison.OrdinalIgnoreCase))
                {
                    var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var clientId = credentials[0];
                    var clientSecret = credentials[1];

                    return Task.FromResult(new IntrospectRequest(clientId, clientSecret, token));
                }
            }

            throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
        }
    }
}
