using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    using System;
    using Codeworx.Identity.Cache;
    using Codeworx.Identity.OAuth;
    using UriBuilder = Codeworx.Identity.UriBuilder;

    public class RefreshTokenTest : IntegrationTestBase
    {
        [Test]
        public async Task SkipRefreshTokenIfOfflineAccessScopeIsMissing_ExpectsOK()
        {
            var tokenResponseData = await GetTokenResponse("openid");

            Assert.IsNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);
        }

        [Test]
        public async Task IncludeRefreshTokenIfOfflineAccessScopeIsPresent_ExpectsOK()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);
        }

        [Test]
        public async Task SkipRefreshTokenOnClientCredentialFlow_ExpectsOK()
        {
            var request = new TokenRequestBuilder()
                                        .WithGrantType("client_credentials")
                                        .WithClientId(Constants.DefaultServiceAccountClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithScopes("openid offline_access")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, tokenResponse.StatusCode);
            var tokenResponseData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());

            Assert.IsNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);
        }

        [Test]
        public async Task SkipRefreshTokenIfGrantTypeToken_ExpectsOK()
        {
            await this.Authenticate();

            var codeRequest = new OAuthAuthorizationRequestBuilder()
                            .WithRedirectUri("https://example.org/redirect")
                            .WithClientId(Constants.DefaultTokenFlowClientId)
                            .WithResponseType("token")
                            .WithScope("openid offline_access")
                            .Build();

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20");
            codeRequest.Append(uriBuilder);

            var response = await this.TestClient.GetAsync(uriBuilder.ToString());

            Assert.AreEqual(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("example.org", response.Headers.Location.Host);
            var query = response.Headers.Location.Fragment.TrimStart('#')
                                        .Split("&")
                                        .ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

            Assert.Contains("access_token", query.Keys);
            Assert.False(query.ContainsKey("refresh_token"));
        }

        [Test]
        public async Task RedeemRefreshCode_ExpectsOK()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode(tokenResponseData.RefreshToken)
                                 .WithClientId(Constants.DefaultCodeFlowClientId)
                                 .WithClientSecret("clientSecret")
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, refreshResponse.StatusCode);

            var refreshResponseData = JsonConvert.DeserializeObject<TokenResponse>(await refreshResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(refreshResponseData.RefreshToken);
            Assert.IsNotNull(refreshResponseData.AccessToken);
            Assert.AreNotEqual(refreshResponseData.AccessToken, tokenResponseData.AccessToken);
            CollectionAssert.AreEquivalent(tokenResponseData.Scope.Split(" "), refreshResponseData.Scope.Split(" "));
        }

        [Test]
        public async Task RedeemRefreshCodeWithExplicitTenantScope_ExpectsOK()
        {
            var scopes = "openid offline_access tenant";
            var tokenResponseData = await GetTokenResponse(scopes);

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                .WithGrantType("refresh_token")
                .WithRefreshCode(tokenResponseData.RefreshToken)
                .WithClientId(Constants.DefaultCodeFlowClientId)
                .WithClientSecret("clientSecret")
                .WithScopes(scopes)
                .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, refreshResponse.StatusCode);

            var refreshResponseData = JsonConvert.DeserializeObject<TokenResponse>(await refreshResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(refreshResponseData.RefreshToken);
            Assert.IsNotNull(refreshResponseData.AccessToken);

            var previousAccessToken = new JwtSecurityToken(tokenResponseData.AccessToken);
            var refreshedAccessToken = new JwtSecurityToken(refreshResponseData.AccessToken);
            Assert.IsTrue(previousAccessToken.Payload.ContainsKey("current_tenant"));
            Assert.IsTrue(refreshedAccessToken.Payload.ContainsKey("current_tenant"));
            Assert.AreEqual(previousAccessToken.Payload["current_tenant"].ToString(), refreshedAccessToken.Payload["current_tenant"].ToString());
        }

        [Test]
        public async Task RedeemRefreshCodeWithExpirationDate_ExpectsOK()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                .WithGrantType("refresh_token")
                .WithRefreshCode(tokenResponseData.RefreshToken)
                .WithClientId(Constants.DefaultCodeFlowClientId)
                .WithClientSecret("clientSecret")
                .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, refreshResponse.StatusCode);

            var refreshResponseData = JsonConvert.DeserializeObject<TokenResponse>(await refreshResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(refreshResponseData.RefreshToken);
            Assert.IsNotNull(refreshResponseData.AccessToken);
            Assert.AreNotEqual(refreshResponseData.AccessToken, tokenResponseData.AccessToken);

            var previousAccessToken = new JwtSecurityToken(tokenResponseData.AccessToken);
            var refreshedAccessToken = new JwtSecurityToken(refreshResponseData.AccessToken);

            Assert.IsTrue(previousAccessToken.ValidFrom < previousAccessToken.ValidTo);
            Assert.IsTrue(refreshedAccessToken.ValidFrom < refreshedAccessToken.ValidTo);

            var previousAccessTokenDifference = previousAccessToken.ValidTo - previousAccessToken.ValidFrom;
            var refreshedAccessTokenDifference = refreshedAccessToken.ValidTo - refreshedAccessToken.ValidFrom;
            Assert.AreEqual(previousAccessTokenDifference, refreshedAccessTokenDifference);
        }

        [Test]
        public async Task RedeemMalformedRefreshCode_ExpectsException()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode("a")
                                 .WithClientId(Constants.DefaultCodeFlowClientId)
                                 .WithClientSecret("clientSecret")
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            Assert.ThrowsAsync<InvalidCacheKeyFormatException>(() => this.TestClient.PostAsync(uriBuilder.ToString(), content));
        }

        [Test]
        public async Task RedeemInvalidRefreshCode_ExpectsBadRequest()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var tokenParts = tokenResponseData.RefreshToken.Split('.');

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode($"{new string('a', tokenParts[0].Length)}.{new string('a', tokenParts[1].Length)}")
                                 .WithClientId(Constants.DefaultCodeFlowClientId)
                                 .WithClientSecret("clientSecret")
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, refreshResponse.StatusCode);

            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await refreshResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("invalid_grant", errorData.Error);
        }

        [Test]
        public async Task RedeemRefreshCodeWithDifferentClientId_ExpectsUnauthorized()
        {
            var tokenResponseData = await GetTokenResponse("openid offline_access");

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode(tokenResponseData.RefreshToken)
                                 .WithClientId(Constants.DefaultCodeFlowPublicClientId)
                                 .WithClientSecret("clientSecret")
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");
            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(await refreshResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("invalid_client", errorResponse.Error);
        }


        [Test]
        public async Task RedeemRefreshCodeWithNarrowScope_ExpectsOk()
        {
            var reducedScope = new[] { "openid", "offline_access", "scope1" };
            var additionalScope = "scope2";
            var initialScope = reducedScope.Append(additionalScope).ToArray();

            var tokenResponseData = await GetTokenResponse(string.Join(" ", initialScope));

            Assert.IsNotNull(tokenResponseData.RefreshToken);
            Assert.IsNotNull(tokenResponseData.AccessToken);

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode(tokenResponseData.RefreshToken)
                                 .WithClientId(Constants.DefaultCodeFlowClientId)
                                 .WithClientSecret("clientSecret")
                                 .WithScopes(string.Join(" ", reducedScope))
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);
            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, refreshResponse.StatusCode);

            var refreshResponseData = JsonConvert.DeserializeObject<TokenResponse>(await refreshResponse.Content.ReadAsStringAsync());

            var actualScope = refreshResponseData.Scope.Split(" ");

            CollectionAssert.AreEquivalent(reducedScope, actualScope);
            CollectionAssert.DoesNotContain(actualScope, additionalScope);
        }


        [Test]
        public async Task RedeemRefreshCodeWithWidenedScope_ExpectsBadRequest()
        {
            var reducedScope = new[] { "openid", "offline_access", "scope1" };
            var additionalScope = "scope2";

            var tokenResponseData = await GetTokenResponse(string.Join(" ", reducedScope));

            var refreshRequest = new TokenRequestBuilder()
                                 .WithGrantType("refresh_token")
                                 .WithRefreshCode(tokenResponseData.RefreshToken)
                                 .WithClientId(Constants.DefaultCodeFlowClientId)
                                 .WithClientSecret("clientSecret")
                                 .WithScopes(string.Join(" ", reducedScope.Append(additionalScope)))
                                 .Build();

            var refreshBody = JsonConvert.SerializeObject(refreshRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(refreshBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");
            var refreshResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, refreshResponse.StatusCode);

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(await refreshResponse.Content.ReadAsStringAsync());

            Assert.AreEqual("invalid_scope", errorResponse.Error);
        }

        private async Task<TokenResponse> GetTokenResponse(string scopes)
        {
            await this.Authenticate();

            var codeRequest = new OAuthAuthorizationRequestBuilder().WithRedirectUri("https://example.org/redirect")
                                                                    .WithClientId(Constants.DefaultCodeFlowClientId).WithResponseType("code")
                                                                    .WithScope(scopes).Build();

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20");
            codeRequest.Append(uriBuilder);

            var response = await this.TestClient.GetAsync(uriBuilder.ToString());

            Assert.AreEqual(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("example.org", response.Headers.Location.Host);
            var query = response.Headers.Location.Query.TrimStart('?').Split("&").ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

            Assert.Contains("code", query.Keys);

            var request = new TokenRequestBuilder().WithGrantType("authorization_code").WithCode(query["code"])
                                                   .WithClientId(Constants.DefaultCodeFlowClientId).WithClientSecret("clientSecret")
                                                   .WithRedirectUri("https://example.org/redirect").Build();

            var body = JsonConvert.SerializeObject(request);

            uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, tokenResponse.StatusCode);
            var tokenResponseData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
            return tokenResponseData;
        }
    }
}