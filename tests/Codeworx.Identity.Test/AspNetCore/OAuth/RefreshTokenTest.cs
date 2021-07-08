using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class RefreshTokenTest : IntegrationTestBase
    {

        [Test]
        public async Task SkipRefreshTokenIfOfflineAccessScopeIsMissing_ExpectsOK()
        {
            await this.Authenticate();

            var codeRequest = new OAuthAuthorizationRequestBuilder()
                            .WithRedirectUri("https://example.org/redirect")
                            .WithClientId(Constants.DefaultCodeFlowClientId)
                            .WithResponseType("code")
                            .WithScope("openid")
                            .Build();

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20");
            codeRequest.Append(uriBuilder);

            var response = await this.TestClient.GetAsync(uriBuilder.ToString());

            Assert.AreEqual(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("example.org", response.Headers.Location.Host);
            var query = response.Headers.Location.Query.TrimStart('?')
                                        .Split("&")
                                        .ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

            Assert.Contains("code", query.Keys);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("authorization_code")
                                        .WithCode(query["code"])
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithRedirectUri("https://example.org/redirect")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
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
        public async Task IncludeRefreshTokenIfOfflineAccessScopeIsPresent_ExpectsOK()
        {
            await this.Authenticate();

            var codeRequest = new OAuthAuthorizationRequestBuilder()
                            .WithRedirectUri("https://example.org/redirect")
                            .WithClientId(Constants.DefaultCodeFlowClientId)
                            .WithResponseType("code")
                            .WithScope("openid offline_access")
                            .Build();

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20");
            codeRequest.Append(uriBuilder);

            var response = await this.TestClient.GetAsync(uriBuilder.ToString());

            Assert.AreEqual(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("example.org", response.Headers.Location.Host);
            var query = response.Headers.Location.Query.TrimStart('?')
                                        .Split("&")
                                        .ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

            Assert.Contains("code", query.Keys);

            var request = new TokenRequestBuilder()
                                        .WithGrantType("authorization_code")
                                        .WithCode(query["code"])
                                        .WithClientId(Constants.DefaultCodeFlowClientId)
                                        .WithClientSecret("clientSecret")
                                        .WithRedirectUri("https://example.org/redirect")
                                        .Build();

            var body = JsonConvert.SerializeObject(request);

            uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("oauth20/token");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            var tokenResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, tokenResponse.StatusCode);
            var tokenResponseData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());

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
    }
}
