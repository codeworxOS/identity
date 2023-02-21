using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.AspNetCore.Binder.Login.OAuth;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Test.Provider;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Login.OAuth
{
    public class OAuthLoginConfigurationClientAuthenticationTests
    {

        [Test]
        public async Task TestClientAuthenticationHeader()
        {
            var config = new OAuthLoginConfiguration
            {
                ClientId = "abc",
                ClientSecret = "def",
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "",
            };

            var data = new Dictionary<string, string>() {
                { "client_id", "abc" },
                { "grant_type", "authorization_code" },
            };

            var message = ExternalOAuthTokenService.CreateTokenRequestMessage(config, data);
            var request = await message.Content.ReadAsStringAsync();

            Assert.AreEqual(
                new AuthenticationHeaderValue("Basic", $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("abc:def"))}"),
                message.Headers.Authorization);

            Assert.False(request.Contains("client_secret=def"));
        }

        [Test]
        public async Task TestNoClientAuthenticationHeader()
        {
            var config = new OAuthLoginConfiguration
            {
                ClientId = "abc",
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "",
            };

            var data = new Dictionary<string, string>() {
                { "client_id", "abc" },
                { "grant_type", "authorization_code" },
            };

            var message = ExternalOAuthTokenService.CreateTokenRequestMessage(config, data);
            var request = await message.Content.ReadAsStringAsync();

            Assert.IsNull(message.Headers.Authorization);
            Assert.False(request.Contains("client_secret="));
        }

        [Test]
        public async Task TestNoClientAuthenticationBody()
        {
            var config = new OAuthLoginConfiguration
            {
                ClientId = "abc",
                ClientAuthenticationMode = ClientAuthenticationMode.Body,
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "",
            };

            var data = new Dictionary<string, string>() {
                { "client_id", "abc" },
                { "grant_type", "authorization_code" },
            };

            var message = ExternalOAuthTokenService.CreateTokenRequestMessage(config, data);

            var request = await message.Content.ReadAsStringAsync();

            Assert.IsNull(message.Headers.Authorization);
            Assert.False(request.Contains("client_secret=def"));
        }

        [Test]
        public async Task TestClientAuthenticationBody()
        {
            var config = new OAuthLoginConfiguration
            {
                ClientId = "abc",
                ClientSecret = "def",
                ClientAuthenticationMode = ClientAuthenticationMode.Body,
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "",
            };

            var data = new Dictionary<string, string>() {
                { "client_id", "abc" },
                { "grant_type", "authorization_code" },
            };

            var message = ExternalOAuthTokenService.CreateTokenRequestMessage(config, data);
            var request = await message.Content.ReadAsStringAsync();

            Assert.IsNull(message.Headers.Authorization);
            Assert.True(request.Contains("client_secret=def"));
        }

        [Test]
        public async Task TestAuthorizationParameters()
        {
            var input =
"{" +
"    \"clientId\": \"abc\"," +
"    \"baseUri\": \"https://localhost/oauth\"," +
"    \"authorizationParameters\": { \"resource\": \"res\", \"integer\": 3 }," +
"    \"tokenParameters\":  { \"resource\": \"res\", \"integer\": 3 }" +
"}";

            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthLoginConfiguration>(input);

            var response = new OAuthRedirectResponse(
                config.GetAuthorizationEndpointUri().ToString(),
                config.ClientId,
                "state",
                "nonce",
                "https://localhost/callback",
                config?.Scope?.Split(" ") ?? new string[] { },
                null,
                config.AuthorizationParameters);

            IResponseBinder<OAuthRedirectResponse> binder = new OAuthRedirectResponseBinder();
            var context = new DefaultHttpContext();
            await binder.BindAsync(response, context.Response);

            var uri = new Uri("https://localhost/oauth?client_id=abc&integer=3&nonce=nonce&redirect_uri=https%3A%2F%2Flocalhost%2Fcallback&resource=res&response_type=code&state=state");
            Assert.AreEqual(uri, context.Response.GetTypedHeaders().Location);
        }

        [Test]
        public async Task TestTokenParameters()
        {
            var input =
"{" +
"    \"clientId\": \"abc\"," +
"    \"baseUri\": \"https://localhost/oauth\"," +
"    \"tokenEndpoint\": \"token\"," +
"    \"authorizationParameters\": { \"resource\": \"res\", \"integer\": 3 }," +
"    \"tokenParameters\":  { \"resource\": \"res\", \"integer\": 3 }" +
"}";

            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthLoginConfiguration>(input);

            var data = new Dictionary<string, string>() {
                { "client_id", "abc" },
                { "grant_type", "authorization_code" },
            };

            var message = ExternalOAuthTokenService.CreateTokenRequestMessage(config, data);

            var body = (FormUrlEncodedContent)message.Content;
            var content = await body.ReadAsStringAsync();

            Assert.True(content.Contains("resource=res"));
            Assert.True(content.Contains("integer=3"));
        }

        [Test]
        public async Task TestDeserialzeTokenAndAuthenticationParametersWithNewtonsoft()
        {
            var input =
"{" +
"    \"clientId\": \"abc\"," +
"    \"baseUri\": \"https://localhost/oauth\"," +
"    \"authorizationParameters\": { \"resource\": \"res\", \"integer\": 3 }," +
"    \"tokenParameters\":  { \"resource\": \"res\", \"integer\": 3 }" +
"}";

            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthLoginConfiguration>(input);

            Assert.AreEqual("abc", config.ClientId);
            Assert.IsNotEmpty(config.AuthorizationParameters);
            Assert.IsNotEmpty(config.TokenParameters);

            Assert.AreEqual("res", config.AuthorizationParameters["resource"]);
            Assert.AreEqual(3, config.AuthorizationParameters["integer"]);

            Assert.AreEqual("res", config.TokenParameters["resource"]);
            Assert.AreEqual(3, config.TokenParameters["integer"]);

        }
    }
}
