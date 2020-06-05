using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IOptions<AuthorizationCodeOptions> _options;

        public AuthorizationCodeResponseProcessor(
            IAuthorizationCodeGenerator authorizationCodeGenerator,
            IOptions<AuthorizationCodeOptions> options,
            IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _options = options;
            _cache = cache;
        }

        public async Task<IAuthorizationResponseBuilder> ProcessAsync(IAuthorizationParameters parameters, IdentityData data, IAuthorizationResponseBuilder responseBuilder)
        {
            parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            data = data ?? throw new ArgumentNullException(nameof(data));
            responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));

            if (!parameters.ResponseTypes.Contains(Constants.OAuth.ResponseType.Code))
            {
                return responseBuilder;
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(parameters, _options.Value.Length)
                                                                     .ConfigureAwait(false);

            await _cache.SetAsync(authorizationCode, data, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                    .ConfigureAwait(false);

            return responseBuilder.WithCode(authorizationCode);
        }
    }
}