using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.CodeGenerationResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeGenerator : IAuthorizationCodeGenerator
    {
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IDistributedCache _cache;
        private readonly IAuthorizationCodeCacheKeyBuilder _cacheKeyBuilder;
        private readonly IUserService _userService;
        private static readonly IReadOnlyCollection<char> _allowedCharacters;

        static AuthorizationCodeGenerator()
        {
            var allowedCharacters = new List<char>();

            for (var i = '\u0020'; i < '\u007e'; i++)
            {
                allowedCharacters.Add(i);
            }

            _allowedCharacters = allowedCharacters;
        }

        public AuthorizationCodeGenerator(IOptions<AuthorizationCodeOptions> options, IDistributedCache cache, IAuthorizationCodeCacheKeyBuilder cacheKeyBuilder, IUserService userService)
        {
            this._options = options;
            _cache = cache;
            _cacheKeyBuilder = cacheKeyBuilder;
            _userService = userService;
        }

        #region Public Methods

        public async Task<IAuthorizationCodeGenerationResult> GenerateCode(AuthorizationRequest request, string userIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                throw new ArgumentNullException(nameof(userIdentifier));
            }

            var user = await _userService.GetUserByIdentifierAsync(userIdentifier)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return new AccessDeniedResult(request.RedirectUri, request.State);
            }

            if (!user.OAuthClientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType))
            {
                return new ClientNotAuthorizedResult(request.RedirectUri, request.State);
            }

            var authorizationCodeBuilder = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < _options.Value.Length; i++)
                {
                    var randomByte = new byte[1];
                    rng.GetNonZeroBytes(randomByte);

                    var index = randomByte[0] % _allowedCharacters.Count;
                    var value = _allowedCharacters.ElementAt(index);
                    authorizationCodeBuilder.Append(value);
                }
            }

            var authorizationCode = authorizationCodeBuilder.ToString();

            await _cache.SetStringAsync(_cacheKeyBuilder.Get(request, userIdentifier),
                                        authorizationCode,
                                        new DistributedCacheEntryOptions
                                        {
                                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds)
                                        })
                        .ConfigureAwait(false);

            return new SuccessfulGenerationResult(authorizationCode);
        }

        #endregion
    }
}