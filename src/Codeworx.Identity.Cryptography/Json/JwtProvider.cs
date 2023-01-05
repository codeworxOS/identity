using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Cryptography.Json
{
    public class JwtProvider : ITokenProvider
    {
        private static readonly JwtSecurityTokenHandler _handler;
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;
        private readonly ITokenCache _tokenCache;

        static JwtProvider()
        {
            _handler = new JwtSecurityTokenHandler();
        }

        public JwtProvider(IDefaultSigningKeyProvider defaultSigningKeyProvider, ITokenCache tokenCache)
        {
            _defaultSigningKeyProvider = defaultSigningKeyProvider;
            _tokenCache = tokenCache;
        }

        public Type ConfigurationType { get; } = typeof(JwtConfiguration);

        public string TokenFormat => Constants.Token.Jwt;

        public bool CanHandle(string tokenValue)
        {
            return _handler.CanReadToken(tokenValue);
        }

        public Task<IToken> CreateAsync(TokenType tokenType, object configuration, CancellationToken token = default)
        {
            return Task.FromResult<IToken>(new Jwt(tokenType, _tokenCache, _defaultSigningKeyProvider, configuration as JwtConfiguration));
        }
    }
}