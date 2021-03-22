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

        public virtual async Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri)
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

            var identity = new ClaimsIdentity();

            await AddClaimsAsync(token, identity, Constants.OAuth.AccessTokenName);
            identity.AddClaim(new Claim(Constants.OAuth.AccessTokenName, responseValues.AccessToken));

            if (!string.IsNullOrWhiteSpace(responseValues.IdToken))
            {
                identity.AddClaim(new Claim(Constants.OpenId.IdTokenName, responseValues.IdToken));

                var identityToken = await provider.CreateAsync(null);
                await identityToken.ParseAsync(responseValues.IdToken);
                var identityPayload = await identityToken.GetPayloadAsync();
                await AddClaimsAsync(identityToken, identity, Constants.OpenId.IdTokenName);
            }

            return identity;
        }

        private static async Task AddClaimsAsync(IToken token, ClaimsIdentity identity, string source)
        {
            var payload = await token.GetPayloadAsync();

            foreach (var item in payload)
            {
                if (item.Value is string stringValue)
                {
                    AddClaimIfNotExists(identity, item.Key, stringValue, source);
                }
                else if (item.Value is IEnumerable<object> enumerable)
                {
                    foreach (var value in enumerable)
                    {
                        AddClaimIfNotExists(identity, item.Key, value, source);
                    }
                }
                else
                {
                    AddClaimIfNotExists(identity, item.Key, item.Value, source);
                }
            }
        }

        private static void AddClaimIfNotExists(ClaimsIdentity identity, string key, object value, string source)
        {
            Claim claim = null;

            if (value is string stringValue)
            {
                claim = new Claim(key, stringValue);
            }
            else if (value is IDictionary<string, object> dictionary)
            {
                claim = new Claim(key, JsonConvert.SerializeObject(dictionary), "json");
            }
            else
            {
                claim = new Claim(key, value.ToString());
            }

            claim.Properties.Add(Constants.OAuth.TokenTypeName, source);

            if (!identity.HasClaim(claim.Type, claim.Value))
            {
                identity.AddClaim(claim);
            }
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