using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaLoginTests : IntegrationTestBase
    {
        [Test]
        public async Task TestLoginWithNoMfaRequired_RedirectsToRedirectUrl()
        {
            await this.Authenticate(TestConstants.Users.DefaultAdmin.UserName, TestConstants.Users.DefaultAdmin.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(new Uri("https://example.org/redirect"), authorizationResponse.Headers.Location);
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnUser_RedirectsToMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaRequired.UserName, TestConstants.Users.MfaRequired.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            var mfaUrl = this.GetMfaUrl();
            Assert.AreEqual(mfaUrl, authorizationResponse.Headers.Location);
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnTenant_RedirectsToMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaRequiredOnTenant.UserName, TestConstants.Users.MfaRequiredOnTenant.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            var mfaUrl = this.GetMfaUrl();
            Assert.AreEqual(mfaUrl, authorizationResponse.Headers.Location);
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnClient_RedirectsToMfa()
        {
            await this.Authenticate(TestConstants.Users.DefaultAdmin.UserName, TestConstants.Users.DefaultAdmin.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            var mfaUrl = this.GetMfaUrl();
            Assert.AreEqual(mfaUrl, authorizationResponse.Headers.Location);
        }

        private async Task Authenticate(string userName, string password)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.Value.AccountEndpoint);
            loginRequestBuilder.AppendPath("login");
            loginRequestBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, "https://example.org/redirect");

            var response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                   {"username", userName},
                                   {"password", password}
                               }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });
        }

        private async Task<HttpResponseMessage> GetAuthorizationResponse(string clientId)
        {
            var authorizationRequest = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(clientId)
                .Build();
            var uriBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("openid10");
            authorizationRequest.Append(uriBuilder);
            var authorizationResponse = await this.TestClient.GetAsync(uriBuilder.ToString());
            return authorizationResponse;
        }

        private Uri GetMfaUrl()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var mfaUrlBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            mfaUrlBuilder.AppendPath(options.Value.AccountEndpoint);
            mfaUrlBuilder.AppendPath("login/mfa");
            var mfaUrl = mfaUrlBuilder.ToString();
            return new Uri(mfaUrl);
        }
    }
}
