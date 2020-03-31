using System;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationMiddlewareTests : IntegrationTestBase
    {
        [Fact(Skip = "Implement")]
        public void Invoke_AccessDenied_RedirectWithError()
        {
            // ToDo: The resource owner or authorization server denied the request.
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Invoke_ClientIdentifierInvalid_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId("\u0019")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task Invoke_ClientIdentifierMissing_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(string.Empty)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task Invoke_ClientNotAuthorized_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithResponseType(Identity.OAuth.Constants.ResponseType.Token)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.UnauthorizedClient}", queryParts[0]);
        }

        [Fact]
        public async Task Invoke_EmptyQuery_InformUserNoRedirect()
        {
            await this.Authenticate();

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
        }

        [Fact]
        public async Task Invoke_InvalidParameter_RedirectWithError()
        {
            await this.Authenticate();

            const string InvalidState = "ä";
            var request = new OAuthAuthorizationRequestBuilder().WithState(InvalidState)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(3, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidRequest}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.StateName}", queryParts[1]);
            Assert.Equal($"{Identity.OAuth.Constants.StateName}={InvalidState}", queryParts[2]);
        }

        [Fact]
        public async Task Invoke_RedirectUriInvalid_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri("x:invalidUri")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Invoke_RedirectUriRelative_InformUserNoRedirect()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri("/invalidRedirect")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Invoke_RequiredParameterMissing_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidRequest}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.ResponseTypeName}", queryParts[1]);
        }

        [Fact]
        public async Task Invoke_ScopeUnknown_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(Constants.DefaultCodeFlowClientId)
                                                           .WithScope("unknown")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidScope}", queryParts[0]);
        }

        [Fact]
        public async Task Invoke_ScopeWithInvalidCharacter_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithScope("ä")
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidScope}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.ScopeName}", queryParts[1]);
        }

        [Fact(Skip = "Implement")]
        public void Invoke_ServerError_RedirectWithError()
        {
            // ToDo: The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
            // (This error code is needed because a 500 Internal Server Error HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Fact(Skip = "Implement")]
        public void Invoke_ServerTemporarilyUnavailable_RedirectWithError()
        {
            // ToDo: The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.
            // (This error code is needed because a 503 Service Unavailable HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Invoke_UnsupportedResponseType_RedirectWithError()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder()
                          .WithClientId(Constants.DefaultCodeFlowClientId)
                          .WithResponseType("unsupported")
                          .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.UnsupportedResponseType}", queryParts[0]);
        }

        [Fact]
        public async Task Invoke_ValidRequest_RedirectWithAuthorizationCode()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(Constants.DefaultCodeFlowClientId)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var cache = this.TestServer.Host.Services.GetRequiredService<IDistributedCache>();

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);

            var grantInformation = await cache.GetStringAsync(WebUtility.UrlDecode(queryParts[0].Split("=")[1]));
            Assert.False(string.IsNullOrWhiteSpace(grantInformation));
        }

        [Fact]
        public async Task Invoke_ValidRequestWithoutRedirectUri_RedirectWithAuthorizationCodeToDefaultRedirectUri()
        {
            await this.Authenticate();

            var request = new OAuthAuthorizationRequestBuilder().WithRedirectUri(string.Empty)
                                                           .WithClientId(Constants.DefaultCodeFlowClientId)
                                                           .Build();

            var requestString = this.ToRequestString(request);

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://example.org/redirect", $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");
        }

        private string ToRequestString(OAuthAuthorizationRequest request)
        {
            return $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}" +
                   $"&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}" +
                   $"&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}" +
                   $"&{Identity.OAuth.Constants.ScopeName}={request.Scope}" +
                   $"&{Identity.OAuth.Constants.StateName}={request.State}";
        }
    }
}