using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenService : ITokenService<TokenRequest>
    {
        private readonly IEnumerable<ITokenServiceSelector> _tokenServiceSelectors;

        public TokenService(IEnumerable<ITokenServiceSelector> tokenServiceSelectors)
        {
            _tokenServiceSelectors = tokenServiceSelectors;
        }

        public async Task<TokenResponse> ProcessAsync(TokenRequest request, CancellationToken token = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            foreach (var item in _tokenServiceSelectors)
            {
                if (item.CanProcess(request))
                {
                    return await item.ProcessAsync(request, token).ConfigureAwait(false);
                }
            }

            ErrorResponse.Throw(Constants.OAuth.Error.UnsupportedGrantType);
            return null;
        }
    }
}