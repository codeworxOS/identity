using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationMiddlewareTests : IntegrationTestBase
    {
        [Test()]
        [Ignore("Implement")]
        public void Invoke_AccessDenied_RedirectWithError()
        {
            // ToDo: The resource owner or authorization server denied the request.
            throw new NotImplementedException();
        }

        [Test]
        public async Task Invoke_ClientIdentifierInvalid_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId("\u0019")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.Error.InvalidRequest));
            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.ClientIdName));
        }

        [Test]
        public async Task Invoke_ClientIdentifierMissing_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(string.Empty)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.Error.InvalidRequest));
            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.ClientIdName));
        }

        [Test]
        [Ignore("TODO reimplement")]
        public async Task Invoke_ClientNotAuthorized_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithResponseType(Constants.OAuth.ResponseType.Token)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Fragment, UriFormat.SafeUnescaped).Split("&");
            Assert.AreEqual(1, queryParts.Count());
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.UnauthorizedClient}", queryParts[0]);
        }

        [Test]
        public async Task Invoke_EmptyQuery_InformUserNoRedirect()
        {
            await this.Authenticate();

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.Error.InvalidRequest));
        }

        [Test]
        public async Task Invoke_InvalidParameter_RedirectWithError()
        {
            await this.Authenticate();

            const string InvalidState = "ä";
            var request = new OAuthAuthorizationRequestBuilder().WithState(InvalidState)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location
                .GetComponents(UriComponents.Fragment, UriFormat.SafeUnescaped)
                .Split("&")
                .OrderBy(p => p)
                .ToList();

            Assert.AreEqual(3, queryParts.Count);
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.InvalidRequest}", queryParts[1]);
            Assert.AreEqual($"{Constants.OAuth.ErrorDescriptionName}={Constants.OAuth.StateName}", queryParts[0]);
            Assert.AreEqual($"{Constants.OAuth.StateName}={InvalidState}", queryParts[2]);
        }

        [Test]
        public async Task Invoke_RedirectUriInvalid_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri("x:invalidUri")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.Error.InvalidRequest));
            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.RedirectUriName));
        }

        [Test]
        public async Task Invoke_RedirectUriRelative_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri("/invalidRedirect")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.Error.InvalidRequest));
            Assert.IsTrue(responseHtml.Contains(Constants.OAuth.RedirectUriName));
        }

        [Test]
        public async Task Invoke_RequiredParameterMissing_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().Build();

            var requestString = $"?{Constants.OAuth.ClientIdName}={request.ClientId}&{Constants.OAuth.RedirectUriName}={request.RedirectUri}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location
                .GetComponents(UriComponents.Fragment, UriFormat.SafeUnescaped)
                .Split("&")
                .OrderBy(p => p)
                .ToList();

            Assert.AreEqual(2, queryParts.Count);
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.InvalidRequest}", queryParts[1]);
            Assert.AreEqual($"{Constants.OAuth.ErrorDescriptionName}={Constants.OAuth.ResponseTypeName}", queryParts[0]);
        }

        [Test]
        public async Task Invoke_ScopeUnknown_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                                           .WithScope("unknown")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.AreEqual(1, queryParts.Length);
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.InvalidScope}", queryParts[0]);
        }

        [Test]
        public async Task Invoke_ScopeWithInvalidCharacter_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithScope("ä")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location
                .GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                .Split("&")
                .OrderBy(p => p)
                .ToList();

            Assert.AreEqual(2, queryParts.Count);
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.InvalidRequest}", queryParts[1]);
            Assert.AreEqual($"{Constants.OAuth.ErrorDescriptionName}={Constants.OAuth.ScopeName}", queryParts[0]);
        }

        [Test]
        [Ignore("Implement")]
        public void Invoke_ServerError_RedirectWithError()
        {
            // ToDo: The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
            // (This error code is needed because a 500 Internal Server Error HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("Implement")]
        public void Invoke_ServerTemporarilyUnavailable_RedirectWithError()
        {
            // ToDo: The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.
            // (This error code is needed because a 503 Service Unavailable HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO reimplement")]
        public async Task Invoke_UnsupportedResponseType_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder()
                          .WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                          .WithResponseType("unsupported")
                          .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.AreEqual(1, queryParts.Count());
            Assert.AreEqual($"{Constants.OAuth.ErrorName}={Constants.OAuth.Error.UnsupportedResponseType}", queryParts[0]);
        }

        [Test]
        public async Task Invoke_ValidRequest_RedirectWithAuthorizationCode()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var cache = this.TestServer.Host.Services.GetRequiredService<IDistributedCache>();

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.AreEqual(1, queryParts.Count());

            var grantInformation = await cache.GetStringAsync(WebUtility.UrlDecode(queryParts[0].Split("=")[1].Split('.')[0]));
            Assert.False(string.IsNullOrWhiteSpace(grantInformation));
        }

        [Test]
        public async Task Invoke_ValidRequestWithoutRedirectUri_RedirectWithAuthorizationCodeToDefaultRedirectUri()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri(string.Empty)
                                                           .WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("https://example.org/redirect", $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");
        }

        private string ToRequestString(AuthorizationRequest request)
        {
            return $"?{Constants.OAuth.ClientIdName}={request.ClientId}" +
                   $"&{Constants.OAuth.RedirectUriName}={request.RedirectUri}" +
                   $"&{Constants.OAuth.ResponseTypeName}={request.ResponseType}" +
                   $"&{Constants.OAuth.ScopeName}={request.Scope}" +
                   $"&{Constants.OAuth.StateName}={request.State}";
        }
    }
}