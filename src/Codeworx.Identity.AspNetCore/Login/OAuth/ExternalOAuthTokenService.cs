using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
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

        public async Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri)
        {
            _client.BaseAddress = oauthConfiguration.BaseUri;

            var contentCollection = new Dictionary<string, string>
            {
                { Constants.OAuth.GrantTypeName, Constants.OAuth.GrantType.AuthorizationCode },
                { Constants.OAuth.CodeName, code },
                { Constants.OAuth.RedirectUriName, redirectUri },
                { Constants.OAuth.ClientIdName, oauthConfiguration.ClientId },
            };

            if (oauthConfiguration.ClientSecret != null)
            {
                var encodedSecret = Convert.ToBase64String(new UTF8Encoding().GetBytes($"{oauthConfiguration.ClientId}:{oauthConfiguration.ClientSecret}"));

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedSecret);
            }

            var response = await _client.PostAsync(oauthConfiguration.TokenEndpoint, new FormUrlEncodedContent(contentCollection));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseValues = JsonConvert.DeserializeObject<ResponseType>(responseString);

            var provider = _tokenProviders.First(p => p.TokenType == Constants.Token.Jwt);
            var token = await provider.CreateAsync(null);

            await token.ParseAsync(responseValues.AccessToken);

            var payload = await token.GetPayloadAsync();

            var identity = new ClaimsIdentity();

            foreach (var item in payload)
            {
                if (item.Value is string stringValue)
                {
                    identity.AddClaim(new Claim(item.Key, stringValue));
                }
                else if (item.Value is IEnumerable<string> stringEnumerable)
                {
                    foreach (var value in stringEnumerable)
                    {
                        identity.AddClaim(new Claim(item.Key, value));
                    }
                }
                else
                {
                    identity.AddClaim(new Claim(item.Key, item.Value.ToString()));
                }
            }

            if (oauthConfiguration.RedirectCacheMethod == RedirectCacheMethod.UseNonce)
            {
                var identityToken = await provider.CreateAsync(null);
                await identityToken.ParseAsync(responseValues.IdToken);
                var identityPayload = await identityToken.GetPayloadAsync();
                if (identityPayload.TryGetValue(Constants.OAuth.NonceName, out var nonceValue))
                {
                    identity.AddClaim(new Claim(Constants.OAuth.NonceName, nonceValue.ToString()));
                }
            }

            return identity;
        }

        private class ResponseType
        {
            [JsonProperty(Constants.OAuth.AccessTokenName)]
            public string AccessToken { get; set; }

            [JsonProperty(Constants.OpenId.IdTokenName)]
            public string IdToken { get; set; }

            [JsonProperty(Constants.OAuth.RefreshTokenName)]
            public string RefreshToken { get; set; }

            [JsonProperty(Constants.OAuth.ScopeName)]
            public string Scope { get; set; }
        }
    }
}