using System;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Cryptography.Json
{
    public class JwtProvider : ITokenProvider
    {
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;

        public JwtProvider(IDefaultSigningKeyProvider defaultSigningKeyProvider)
        {
            _defaultSigningKeyProvider = defaultSigningKeyProvider;
        }

        public Type ConfigurationType { get; } = typeof(JwtConfiguration);

        public string TokenType => Constants.Token.Jwt;

        public Task<IToken> CreateAsync(object configuration)
        {
            return Task.FromResult<IToken>(new Jwt(_defaultSigningKeyProvider, configuration as JwtConfiguration));
        }
    }
}