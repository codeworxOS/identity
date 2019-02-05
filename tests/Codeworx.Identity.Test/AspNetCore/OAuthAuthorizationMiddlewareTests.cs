using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore
{
    public class OAuthAuthorizationMiddlewareTests : IntegrationTestBase
    {
        [Fact]
        public async Task Inform_User_On_Redirect_Uri_Missing_No_Redirect_Test()
        {
            var emptyUri = string.Empty;
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={emptyUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Inform_User_On_Redirect_Uri_Invalid_No_Redirect_Test()
        {
            const string InvalidUri = "x:invalidUri";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={InvalidUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Inform_User_On_Redirect_Uri_Must_Not_Be_Relative_No_Redirect_Test()
        {
            var invalidRedirect = "/invalidRedirect";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={invalidRedirect}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Inform_User_On_Client_Identifier_Missing_No_Redirect_Test()
        {
            var emptyClientId = string.Empty;
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={emptyClientId}&{OAuth.Constants.RedirectUriName}={request.RedirectUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task Inform_User_On_Client_Identifier_Invalid_No_Redirect_Test()
        {
            var invalidClientId = "\u0019";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={invalidClientId}&{OAuth.Constants.RedirectUriName}={request.RedirectUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task InvalidRequest_When_Query_Empty_Test()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(OAuth.Constants.Error.InvalidRequest, responseHtml);
        }

        [Fact]
        public async Task InvalidRequest_When_Missing_Required_Parameter_Test()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={request.RedirectUri}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(new Uri($"{request.RedirectUri}?{OAuth.Constants.ErrorName}={OAuth.Constants.Error.InvalidRequest}&{OAuth.Constants.ErrorDescriptionName}={OAuth.Constants.ResponseTypeName}"), response.Headers.Location);
        }

        [Fact]
        public async Task InvalidRequest_When_Includes_Invalid_Parameter_Test()
        {
            const string InvalidState = "ä";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={request.RedirectUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}&{OAuth.Constants.StateName}={InvalidState}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(new Uri($"{request.RedirectUri}?{OAuth.Constants.ErrorName}={OAuth.Constants.Error.InvalidRequest}&{OAuth.Constants.ErrorDescriptionName}={OAuth.Constants.StateName}&{OAuth.Constants.StateName}={InvalidState}"), response.Headers.Location);
        }

        [Fact]
        public void Unauthorized_Client_Test()
        {
            // ToDo: The client is not authorized to request an authorization code using this method.
            throw new NotImplementedException();
        }

        [Fact]
        public void Access_Denied_Test()
        {
            // ToDo: The resource owner or authorization server denied the request.
            throw new NotImplementedException();
        }

        [Fact]
        public void Unsupported_Response_Type_Test()
        {
            // ToDo: The authorization server does not support obtaining an authorization code using this method.
            throw new NotImplementedException();
        }

        [Fact]
        public void Invalid_Scope_Test()
        {
            // ToDo: The requested scope is invalid, unknown, or malformed.
            // Note: Invalid and malformed are already tested when creating an authorization request.
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Invalid_Scope_When_Invalid_Character_Test()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{OAuth.Constants.ClientIdName}={request.ClientId}&{OAuth.Constants.RedirectUriName}={request.RedirectUri}&{OAuth.Constants.ResponseTypeName}={request.ResponseType}&{OAuth.Constants.ScopeName}=ä";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(new Uri($"{request.RedirectUri}?{OAuth.Constants.ErrorName}={OAuth.Constants.Error.InvalidScope}&{OAuth.Constants.ErrorDescriptionName}={OAuth.Constants.ScopeName}"), response.Headers.Location);
        }

        [Fact]
        public void Server_Error_Test()
        {
            // ToDo: The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
            // (This error code is needed because a 500 Internal Server Error HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Fact]
        public void Temporarily_Unavailable_Test()
        {
            // ToDo: The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.
            // (This error code is needed because a 503 Service Unavailable HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }
    }
}
