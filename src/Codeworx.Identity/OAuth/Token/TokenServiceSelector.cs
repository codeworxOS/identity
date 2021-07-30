using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenServiceSelector<TTokenRequest> : ITokenServiceSelector
        where TTokenRequest : TokenRequest
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenServiceSelector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanProcess(TokenRequest request) => request is TTokenRequest;

        public async Task<TokenResponse> ProcessAsync(TokenRequest request)
        {
            if (!(request is TTokenRequest tokenRequest))
            {
                throw new ArgumentException("Wrong request type.", nameof(request));
            }

            var target = _serviceProvider.GetRequiredService<ITokenService<TTokenRequest>>();
            return await target.ProcessAsync(tokenRequest);
        }
    }
}
