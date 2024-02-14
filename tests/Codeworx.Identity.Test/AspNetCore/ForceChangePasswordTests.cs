﻿using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Test.AspNetCore
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.OAuth;
    using Codeworx.Identity.Test.Provider;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using NUnit.Framework;

    public class ForceChangePasswordTests : IntegrationTestBase
    {
        [Test]
        public async Task LoginWithForcePasswordChangeShouldRedirectToPasswordChange()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IdentityServerOptions>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.AccountEndpoint);
            loginRequestBuilder.AppendPath("login");

            var response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                   {"username", TestConstants.Users.ForceChangePassword.UserName},
                                   {"password", TestConstants.Users.ForceChangePassword.Password}
                               }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));

            Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);

            var redirectLocation = response.Headers.Location;
            var expectedRedirectTarget = options.AccountEndpoint + "/change-password";
            Assert.True(redirectLocation.LocalPath.Contains(expectedRedirectTarget));

            var changePasswordContent = new FormUrlEncodedContent(new Dictionary<string, string>
                                             {
                                                 {"username", TestConstants.Users.ForceChangePassword.UserName},
                                                 {"current-password", TestConstants.Users.ForceChangePassword.Password},
                                                 {"password", "aaAAbb11!!"},
                                                 {"confirm-password", "aaAAbb11!!"},
                                             });

            changePasswordContent.Headers.Add(HeaderNames.Cookie, new[] { authenticationCookie });

            var changePasswordResponse = await this.TestClient.PostAsync(redirectLocation, changePasswordContent);

            response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                   {"username", TestConstants.Users.ForceChangePassword.UserName},
                                   {"password", "aaAAbb11!!"}
                               }));

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(new Uri("https://localhost/account/me"), response.Headers.Location);
        }

        [Test]
        public async Task LoginWithForcePasswordChangeShouldNotReturnTokens()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IdentityServerOptions>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.AccountEndpoint);
            loginRequestBuilder.AppendPath("login");

            var response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                   {"username", TestConstants.Users.ForceChangePassword.UserName},
                                   {"password", TestConstants.Users.ForceChangePassword.Password}
                               }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                                                .Build();

            var requestString = this.ToRequestString(request);

            var authorizationResponse = await this.TestClient.GetAsync(options.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual($"{options.AccountEndpoint}/change-password", authorizationResponse.Headers.Location.AbsolutePath);
        }

        [Test]
        public async Task LoginWithForcePasswordChangeAndExternalLoginShouldNotShowPasswordChangeForm()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IdentityServerOptions>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath("/test-setup/windowslogin");

            var response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"username", TestConstants.Users.ForceChangePassword.UserName},
                                   {"sid", TestConstants.Users.ForceChangePassword.WindowsSid}
                               }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookies);

            loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.AccountEndpoint);
            loginRequestBuilder.AppendPath("winlogin");
            loginRequestBuilder.AppendPath(TestConstants.LoginProviders.ExternalWindowsProvider.Id);
            loginRequestBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, $"{options.AccountEndpoint}/me");

            response = await this.TestClient.GetAsync(loginRequestBuilder.ToString());

            response.Headers.TryGetValues(HeaderNames.SetCookie, out cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });

            var request = new OAuthAuthorizationRequestBuilder()
                                .WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                .Build();

            var requestString = this.ToRequestString(request);

            var authorizationResponse = await this.TestClient.GetAsync(options.OauthAuthorizationEndpoint + requestString);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual($"/redirect", authorizationResponse.Headers.Location.AbsolutePath);
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
