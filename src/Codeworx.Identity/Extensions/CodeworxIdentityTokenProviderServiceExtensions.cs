using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;
using Newtonsoft.Json;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityTokenProviderServiceExtensions
    {
        public static async Task<IToken> CreateAccessTokenAsync(this ITokenProviderService tokenProviderService, IClientRegistration client, CancellationToken token = default)
        {
            object parameter = null;
            var tokenFormat = client.AccessTokenType ?? Constants.Token.Jwt;
            if (client.AccessTokenTypeConfiguration != null)
            {
                var parameterType = tokenProviderService.GetParameterType(tokenFormat);

                parameter = JsonConvert.DeserializeObject(client.AccessTokenTypeConfiguration, parameterType);
            }

            var result = await tokenProviderService.CreateTokenAsync(tokenFormat, TokenType.AccessToken, parameter, token).ConfigureAwait(false);
            return result;
        }

        public static async Task<IToken> CreateIdentityTokenAsync(this ITokenProviderService tokenProviderService, IClientRegistration client, CancellationToken token = default)
        {
            var result = await tokenProviderService.CreateTokenAsync(Constants.Token.Jwt, TokenType.IdToken, null, token).ConfigureAwait(false);
            return result;
        }

        public static async Task<IToken> CreateRefreshTokenAsync(this ITokenProviderService tokenProviderService, CancellationToken token = default)
        {
            var result = await tokenProviderService.CreateTokenAsync(Constants.Token.Reference, TokenType.RefreshToken, null, token).ConfigureAwait(false);
            return result;
        }
    }
}