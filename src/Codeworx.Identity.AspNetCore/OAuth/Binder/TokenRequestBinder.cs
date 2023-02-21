using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Response;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class TokenRequestBinder : IRequestBinder<TokenRequest>
    {
        private readonly IEnumerable<ITokenRequestBindingSelector> _selectors;

        public TokenRequestBinder(IEnumerable<ITokenRequestBindingSelector> selectors)
        {
            _selectors = selectors;
        }

        public async Task<TokenRequest> BindAsync(HttpRequest request)
        {
            if (!HttpMethods.IsPost(request.Method))
            {
                throw new ErrorResponseException<MethodNotSupportedResponse>(new MethodNotSupportedResponse());
            }

            if (!request.HasFormContentType)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest);
            }

            if (!request.Form.TryGetValue(Constants.OAuth.GrantTypeName, out var grantTypes) || grantTypes.Count != 1)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.GrantTypeName);
            }

            foreach (var item in _selectors)
            {
                if (item.GrantType == grantTypes[0])
                {
                    return await item.BindAsync(request);
                }
            }

            ErrorResponse.Throw(Constants.OAuth.Error.UnsupportedGrantType);
            return null;
        }
    }
}
