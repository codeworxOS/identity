using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    using Codeworx.Identity.OAuth;

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
            var tokenResponseData = await GetTokenResponse(string.Join(" ", reducedScope.Append(additionalScope)));

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

            var actualScope = refreshResponseData.Scope;
            foreach (var scope in reducedScope)
            {
                Assert.True(actualScope.Contains(scope));
            }

            Assert.False(actualScope.Contains(additionalScope));
        }


        [Test]
        public async Task RedeemRefreshCodeWithWidenedScope_ExpectsUnauthorized()
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
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

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