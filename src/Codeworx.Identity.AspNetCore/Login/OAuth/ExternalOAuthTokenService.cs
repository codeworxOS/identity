using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore
{
    public class ExternalOAuthTokenService : IExternalOAuthTokenService
    {
        private readonly HttpClient _client;
        private readonly JsonWebTokenHandler _jwtHandler;
        private readonly ISigningDataProvider _signingDataProvider;

        public ExternalOAuthTokenService(HttpClient client, ISigningDataProvider signingDataProvider = null)
        {
            _client = client;
            _signingDataProvider = signingDataProvider;
            _jwtHandler = new JsonWebTokenHandler();
        }

        public static Task AppendClaimsAsync(JsonWebToken token, ClaimsIdentity identity, string source)
        {
            foreach (var item in token.Claims)
            {
                AddClaimIfNotExists(identity, item, source);
            }

            return Task.CompletedTask;
        }

        public static async Task<HttpRequestMessage> CreateTokenRequestMessageAsync(OAuthLoginConfiguration oauthConfiguration, IDictionary<string, string> content, CancellationToken token, ISigningDataProvider signingDataProvider = null)
        {
            var tokenEndpointUri = oauthConfiguration.GetTokenEndpointUri();
            var message = new HttpRequestMessage(HttpMethod.Post, tokenEndpointUri);

            var body = new Dictionary<string, string>(content);
            foreach (var item in oauthConfiguration.TokenParameters)
            {
                body.Add(item.Key, $"{item.Value}");
            }

            switch (oauthConfiguration.ClientAuthenticationMode)
            {
                case ClientAuthenticationMode.Header:
                    if (oauthConfiguration.ClientSecret != null)
                    {
                        var encodedSecret = Convert.ToBase64String(new UTF8Encoding().GetBytes($"{oauthConfiguration.ClientId}:{oauthConfiguration.ClientSecret}"));
                        message.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedSecret);
                    }

                    break;
                case ClientAuthenticationMode.Body:
                    if (!body.ContainsKey(Constants.OAuth.ClientIdName))
                    {
                        body.Add(Constants.OAuth.ClientIdName, oauthConfiguration.ClientId);
                    }

                    if (!body.ContainsKey(Constants.OAuth.ClientSecretName))
                    {
                        body.Add(Constants.OAuth.ClientSecretName, oauthConfiguration.ClientSecret);
                    }

                    break;
                case ClientAuthenticationMode.JwtSymmetric:
                    if (oauthConfiguration.SigningKey != null)
                    {
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(oauthConfiguration.SigningKey));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                        if (!body.ContainsKey(Constants.OAuth.ClientIdName))
                        {
                            body.Add(Constants.OAuth.ClientIdName, oauthConfiguration.ClientId);
                        }

                        var assertion = GetToken(oauthConfiguration, credentials);
                        body.Add(Constants.OpenId.Client.AssertionTypeParameter, Constants.OpenId.Client.AssertionType.JwtBearer);
                        body.Add(Constants.OpenId.Client.AssertionParameter, assertion);
                    }

                    break;
                case ClientAuthenticationMode.JwtAsymmetric:
                    if (oauthConfiguration.SigningKey != null)
                    {
                        if (signingDataProvider == null)
                        {
                            throw new CertificateNotFoundException("SigningDataProvider not registered");
                        }

                        using (var data = await signingDataProvider.GetSigningDataAsync(oauthConfiguration.SigningKey, token).ConfigureAwait(false))
                        {
                            if (!body.ContainsKey(Constants.OAuth.ClientIdName))
                            {
                                body.Add(Constants.OAuth.ClientIdName, oauthConfiguration.ClientId);
                            }

                            var assertion = GetToken(oauthConfiguration, data.Credentials);
                            body.Add(Constants.OpenId.Client.AssertionTypeParameter, Constants.OpenId.Client.AssertionType.JwtBearer);
                            body.Add(Constants.OpenId.Client.AssertionParameter, assertion);
                        }
                    }

                    break;
                default:
                    throw new NotSupportedException($"The client authentication mode {oauthConfiguration.ClientAuthenticationMode} is not supported");
            }

            message.Content = new FormUrlEncodedContent(body);

            return message;
        }

        public virtual async Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri, CancellationToken token)
        {
            var contentCollection = new Dictionary<string, string>
            {
                { Constants.OAuth.GrantTypeName, Constants.OAuth.GrantType.AuthorizationCode },
                { Constants.OAuth.CodeName, code },
                { Constants.OAuth.RedirectUriName, redirectUri },
                { Constants.OAuth.ClientIdName, oauthConfiguration.ClientId },
            };

            return await GetIdentityAsync(oauthConfiguration, contentCollection, token);
        }

        public virtual async Task<ClaimsIdentity> RefreshAsync(OAuthLoginConfiguration oauthConfiguration, string refreshToken, CancellationToken token)
        {
            var contentCollection = new Dictionary<string, string>
            {
                { Constants.OAuth.GrantTypeName, Constants.OAuth.GrantType.RefreshToken },
                { Constants.OAuth.RefreshTokenName, refreshToken },
                { Constants.OAuth.ClientIdName, oauthConfiguration.ClientId },
            };

            return await GetIdentityAsync(oauthConfiguration, contentCollection, token);
        }

        protected virtual async Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, IDictionary<string, string> contentCollection, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await CreateTokenResponse(oauthConfiguration, contentCollection, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseValues = JsonConvert.DeserializeObject<ResponseType>(responseString);

            ////var provider = _tokenProviders.First(p => p.TokenType == Constants.Token.Jwt);
            ////var token = await provider.CreateAsync(null);
            string tokenName = null;
            JsonWebToken token = null;

            switch (oauthConfiguration.ClaimSource)
            {
                case ClaimSource.AccessToken:
                    tokenName = Constants.OAuth.AccessTokenName;
                    token = _jwtHandler.ReadJsonWebToken(responseValues.AccessToken);
                    break;
                case ClaimSource.IdToken:
                    tokenName = Constants.OpenId.IdTokenName;
                    token = _jwtHandler.ReadJsonWebToken(responseValues.IdToken);
                    break;
                default:
                    throw new NotSupportedException($"Claim source {oauthConfiguration.ClaimSource} not supported!");
            }

            var identity = new ClaimsIdentity();

            await AppendClaimsAsync(token, identity, tokenName);
            identity.AddClaim(new Claim(Constants.OAuth.AccessTokenName, responseValues.AccessToken));

            if (!string.IsNullOrWhiteSpace(responseValues.IdToken))
            {
                identity.AddClaim(new Claim(Constants.OpenId.IdTokenName, responseValues.IdToken));

                if (oauthConfiguration.ClaimSource != ClaimSource.IdToken)
                {
                    var identityToken = _jwtHandler.ReadJsonWebToken(responseValues.IdToken);
                    await AppendClaimsAsync(identityToken, identity, Constants.OpenId.IdTokenName);
                }
            }

            if (!string.IsNullOrWhiteSpace(responseValues.RefreshToken))
            {
                identity.AddClaim(new Claim(Constants.OAuth.RefreshTokenName, responseValues.RefreshToken));
            }

            return identity;
        }

        private static void AddClaimIfNotExists(ClaimsIdentity identity, Claim claim, string source)
        {
            var newClaim = claim.Clone();
            newClaim.Properties.Add(Constants.OAuth.TokenTypeName, source);

            if (!identity.HasClaim(claim.Type, claim.Value))
            {
                identity.AddClaim(claim);
            }
        }

        private static string GetToken(OAuthLoginConfiguration oauthConfiguration, SigningCredentials credentials)
        {
            var handler = new JsonWebTokenHandler();

            var now = DateTime.UtcNow;

            var descriptor = new SecurityTokenDescriptor
            {
                ////AdditionalHeaderClaims = new Dictionary<string, object>
                ////{
                ////    { Constants.Claims.X5t, "CLp/oelcGjFEFFMqLJ+S12EC1Uc=" },
                ////},
                Issuer = oauthConfiguration.ClientId,
                Audience = oauthConfiguration.GetTokenEndpointUri().ToString(),
                Claims = new Dictionary<string, object>
                {
                    { Constants.Claims.Subject, oauthConfiguration.ClientId },
                    { Constants.Claims.Jti, Guid.NewGuid().ToString("N") },
                },
                Expires = now.AddMinutes(5),
                IssuedAt = now,
                NotBefore = now,
                SigningCredentials = credentials,
            };

            return handler.CreateToken(descriptor);
        }

        private async Task<HttpResponseMessage> CreateTokenResponse(OAuthLoginConfiguration oauthConfiguration, IDictionary<string, string> contentCollection, CancellationToken token)
        {
            var request = await CreateTokenRequestMessageAsync(oauthConfiguration, contentCollection, token, _signingDataProvider);
            var response = await _client.SendAsync(request);
            return response;
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