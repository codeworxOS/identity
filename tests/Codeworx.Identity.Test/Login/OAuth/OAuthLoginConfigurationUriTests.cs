using System;
using Codeworx.Identity.Login.OAuth;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Login.OAuth
{
    public class OAuthLoginConfigurationUriTests
    {

        [Test]
        public void TestEmptyAuthorizationEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                AuthorizationEndpoint = "",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth"), config.GetAuthorizationEndpointUri());
        }

        [Test]
        public void TestNullAuthorizationEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                AuthorizationEndpoint = null,
            };

            Assert.AreEqual(new Uri("https://localhost/oauth"), config.GetAuthorizationEndpointUri());
        }

        [Test]
        public void TestFullAuthorizationEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/whatever"),
                AuthorizationEndpoint = "https://localhost/oauth/authorize",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/authorize"), config.GetAuthorizationEndpointUri());
        }

        [Test]
        public void TestAbsoluteAuthorizationEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                AuthorizationEndpoint = "/authorize",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/authorize"), config.GetAuthorizationEndpointUri());
        }
        [Test]
        public void TestRelativeAuthorizationEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                AuthorizationEndpoint = "authorize",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/authorize"), config.GetAuthorizationEndpointUri());
        }



        [Test]
        public void TestEmptyTokenbEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth"), config.GetTokenEndpointUri());
        }

        [Test]
        public void TestNullTokenEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = null,
            };

            Assert.AreEqual(new Uri("https://localhost/oauth"), config.GetTokenEndpointUri());
        }

        [Test]
        public void TestFullTokenEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/whatever"),
                TokenEndpoint = "https://localhost/oauth/token",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/token"), config.GetTokenEndpointUri());
        }

        [Test]
        public void TestAbsoluteTokenEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "/token",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/token"), config.GetTokenEndpointUri());
        }
        [Test]
        public void TestRelativeTokenEndpointWithBaseUrl()
        {
            var config = new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/oauth"),
                TokenEndpoint = "token",
            };

            Assert.AreEqual(new Uri("https://localhost/oauth/token"), config.GetTokenEndpointUri());
        }
    }
}
