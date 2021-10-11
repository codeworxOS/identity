using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Login.OAuth;
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
    }
}
