using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Token;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore
{
    public class ExternalOAuthTokenService : IExternalOAuthTokenService
    {
        private readonly HttpClient _client;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;

        public ExternalOAuthTokenService(HttpClient client, IEnumerable<ITokenProvider> tokenProviders)
        {
            _client = client;
            _tokenProviders = tokenProviders;
        }

        public async Task<string> GetUserIdAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri)
        {
            _client.BaseAddress = oauthConfiguration.BaseUri;

            var contentCollection = new Dictionary<string, string>
            {
                { Identity.OAuth.Constants.GrantTypeName, Identity.OAuth.Constants.GrantType.AuthorizationCode },
                { Identity.OAuth.Constants.CodeName, code },
                { Identity.OAuth.Constants.RedirectUriName, redirectUri },
                { Identity.OAuth.Constants.ClientIdName, oauthConfiguration.ClientId },
            };

            if (oauthConfiguration.ClientSecret != null)
            {
                contentCollection.Add(Identity.OAuth.Constants.ClientSecretName, oauthConfiguration.ClientSecret);
            }

            var response = await _client.PostAsync(oauthConfiguration.TokenEndpoint, new FormUrlEncodedContent(contentCollection));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseValues = JsonConvert.DeserializeObject<ResponseType>(responseString);

            var provider = _tokenProviders.First(p => p.TokenType == Constants.Token.Jwt);
            var token = await provider.CreateAsync(null);

            await token.ParseAsync(responseValues.AccessToken);

            var payload = await token.GetPayloadAsync();

            payload.TryGetValue(Identity.OAuth.Constants.ReservedClaims.UserId, out var userId);

            return userId?.ToString();
        }

        private class ResponseType
        {
            [JsonProperty(Identity.OAuth.Constants.AccessTokenName)]
            public string AccessToken { get; set; }

            [JsonProperty(Identity.OAuth.Constants.RefreshTokenName)]
            public string RefreshToken { get; set; }

            [JsonProperty(Identity.OAuth.Constants.ScopeName)]
            public string Scope { get; set; }
        }
    }
}