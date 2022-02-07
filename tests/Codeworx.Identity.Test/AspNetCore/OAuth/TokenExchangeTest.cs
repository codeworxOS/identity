using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class TokenExchangeTest : IntegrationTestBase
    {
        [Test]
        public async Task GetOnBehalfOfToken_ForConfidentialClient_WithoutRefreshToken_ExpectsOK()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithAudience(Constants.DefaultCodeFlowPublicClientId)
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
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
        public async Task GetOnBehalfOfToken_ForConfidentialClient_UnknownAudience_ExpectsUnauthorized()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithAudience("abcdefg")
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, tokenResponse.StatusCode);

            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await tokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("invalid_target", errorData.Error);
        }

        [Test]
        public async Task GetOnBehalfOfToken_ForConfidentialClient_WrongClientIdForToken_ExpectsUnauthorized()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultTokenFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithAudience(Constants.DefaultCodeFlowPublicClientId)
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, tokenResponse.StatusCode);

            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await tokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("invalid_grant", errorData.Error);
        }


        [Test]
        public async Task GetOnBehalfOfToken_ForConfidentialClient_ExpectsUnauthorized()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithAudience(Constants.DefaultCodeFlowPublicClientId)
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, tokenResponse.StatusCode);

        }

        [Test]
        public async Task GetOnBehalfOfToken_IdToken_ExpectsOK()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithAudience(Constants.DefaultCodeFlowPublicClientId)
                                        .WithRequestedTokenType(Constants.TokenExchange.TokenType.IdToken)
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var response = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

            Assert.IsNull(tokenResponse.AccessToken);
            Assert.IsNotNull(tokenResponse.IdentityToken);
        }

    [Test]
        public async Task GetOnBehalfOfToken_IdToken_And_AccessToken_ExpectsOK()
        {
            var rootScope = "openid scope1";

            var token = await GetTokenResponse(rootScope);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("urn:ietf:params:oauth:grant-type:token-exchange")
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithAudience(Constants.DefaultCodeFlowPublicClientId)
                                        .WithRequestedTokenType(Constants.TokenExchange.TokenType.IdToken + " " + Constants.TokenExchange.TokenType.AccessToken)
                                        .WithSubjectToken(token.AccessToken)
                                        .WithScopes("openid scope2")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var response = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(tokenResponse.AccessToken);
            Assert.IsNotNull(tokenResponse.IdentityToken);
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