using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Token
{
    // EventIds 155xx
    public partial class TokenProviderService : ITokenProviderService
    {
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly ILogger<TokenProviderService> _logger;

        public TokenProviderService(IEnumerable<ITokenProvider> tokenProviders, ILogger<TokenProviderService> logger)
        {
            _tokenProviders = tokenProviders;
            _logger = logger;
        }

        [LoggerMessage(
            Level = LogLevel.Error,
            EventId = 15501,
            Message = "The privder for token type {tokenType} was not found.")]
        public static partial void LogTokenProviderNotFound(ILogger logger, string tokenType);

        [LoggerMessage(
            Level = LogLevel.Error,
            EventId = 15502,
            Message = "The the token format for the provided token is invalid.")]
        public static partial void LogTokenFormatInvalid(ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Trace,
            EventId = 15503,
            Message = "The the token format for the token {token} is invalid.")]
        public static partial void LogTokenFormatInvalidTrace(ILogger logger, string token);

        public async Task<IToken> CreateTokenAsync(string tokenFormat, TokenType tokenType, object parameter, CancellationToken token = default)
        {
            var provider = GetTokenProvider(tokenFormat);
            var result = await provider.CreateAsync(tokenType, parameter);
            return result;
        }

        public Type GetParameterType(string tokenFormat)
        {
            var tokenProvider = GetTokenProvider(tokenFormat);
            return tokenProvider.ConfigurationType;
        }

        public string GetTokenFormat(string tokenValue)
        {
            foreach (var item in _tokenProviders)
            {
                if (item.CanHandle(tokenValue))
                {
                    return item.TokenFormat;
                }
            }

            LogTokenFormatInvalidTrace(_logger, tokenValue);
            LogTokenFormatInvalid(_logger);
            throw new InvalidTokenFormatException();
        }

        private ITokenProvider GetTokenProvider(string tokenFormat)
        {
            var provider = _tokenProviders.FirstOrDefault(p => p.TokenFormat == tokenFormat);
            if (provider == null)
            {
                LogTokenProviderNotFound(_logger, tokenFormat);
                throw new TokenProviderNotFoundException(tokenFormat);
            }

            return provider;
        }
    }
}
