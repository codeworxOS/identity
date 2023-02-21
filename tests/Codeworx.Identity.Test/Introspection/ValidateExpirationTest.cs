using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Introspection
{
    public class ValidateExpirationTest : IntegrationTestBase
    {
        [Test]
        public async Task CheckIfIntrospectionExpirationIsSameAsTokenResponse()
        {
            await Authenticate();

            var builder = new OpenIdAuthorizationRequestBuilder()
                                .WithClientId(TestConstants.Clients.DefaultTokenFlowClientId)
                                .WithResponseType("token id_token")
                                .WithScope("openid");

            var urlBuilder = new UriBuilder(this.TestServer.BaseAddress);
            urlBuilder.AppendPath("openid10");

            builder.Build().Append(urlBuilder);

            var requestUrl = urlBuilder.ToString();

            var response = await this.TestClient.GetAsync(requestUrl);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);

            var values = response.Headers.Location.Fragment.TrimStart('#').Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0], p => p[1]);

            Assert.True(values.ContainsKey(Constants.OAuth.AccessTokenName));
            Assert.True(values.ContainsKey(Constants.OpenId.IdTokenName));

            var token = new JsonWebToken(values[Constants.OpenId.IdTokenName]);
            var tokenValidity = token.ValidTo;

            urlBuilder = new UriBuilder(this.TestServer.BaseAddress);
            urlBuilder.AppendPath("oauth20/introspect");

            var content = new FormUrlEncodedContent(
                                    new Dictionary<string, string>
                                    {
                                        {Constants.OAuth.TokenName,values[Constants.OAuth.AccessTokenName] }
                                    });

            var auth = $"{TestConstants.Clients.DefaultBackendClientId}:{TestConstants.Clients.DefaultBackendClientSecret}";

            this.TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))}");

            var introspectResponse = await TestClient.PostAsync(urlBuilder.ToString(), content);

            Assert.AreEqual(HttpStatusCode.OK, introspectResponse.StatusCode);

            var introspectContent = await introspectResponse.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

            var expiration = DateTimeOffset.FromUnixTimeSeconds(introspectContent["exp"].GetInt32());

            Assert.AreEqual(expiration, new DateTimeOffset(tokenValidity, TimeSpan.Zero));
        }
    }
}
