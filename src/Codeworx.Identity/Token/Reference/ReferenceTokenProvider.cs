using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;

namespace Codeworx.Identity.Token.Reference
{
    public class ReferenceTokenProvider : ITokenProvider
    {
        private readonly ITokenCache _tokenCache;

        public ReferenceTokenProvider(ITokenCache tokenCache)
        {
            _tokenCache = tokenCache;
        }

        public Type ConfigurationType => null;

        public string TokenFormat => Constants.Token.Reference;

        public bool CanHandle(string tokenValue)
        {
            var split = tokenValue.Split('.');
            return split.Length == 2 && split[0].Length == 32;
        }

        public Task<IToken> CreateAsync(TokenType tokenType, object configuration, CancellationToken token = default)
        {
            return Task.FromResult<IToken>(new ReferenceToken(tokenType, _tokenCache));
        }
    }
}
