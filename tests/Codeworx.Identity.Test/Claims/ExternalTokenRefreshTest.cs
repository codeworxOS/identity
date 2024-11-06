using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NUnit.Framework;
using static QRCoder.PayloadGenerator;

namespace Codeworx.Identity.Test.Claims
{
    public class ExternalTokenRefreshTest : IntegrationTestBase
    {
        [Test]
        public async Task RefreshTokenWithExternalTokenScopeOnWindowsProvider_ExpectsNoScopeInResponse()
        {
            await AuthenticateWindows();

            await using (var scope = this.TestServer.Services.CreateAsyncScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var linkService = scope.ServiceProvider.GetRequiredService<ILinkUserService>();
                var user = await userService.GetUserByIdAsync(TestConstants.Users.DefaultAdmin.UserId);
                await linkService.LinkUserAsync(user, new DummyLoginData(TestConstants.Users.DefaultAdmin.WindowsSid));
            }

            var responseLogin = await this.TestClient.GetAsync($"https://localhost/account/winlogin/{TestConstants.LoginProviders.ExternalWindowsProvider.Id}?returnUrl=/account/me");

            responseLogin.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });

            var builder = new OpenIdAuthorizationRequestBuilder()
                                .WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                .WithResponseType("code")
                                .WithScope("openid offline_access external_token");

            var urlBuilder = new UriBuilder(this.TestServer.BaseAddress);
            urlBuilder.AppendPath("openid10");

            builder.Build().Append(urlBuilder);

            var requestUrl = urlBuilder.ToString();

            var response = await this.TestClient.GetAsync(requestUrl);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);

            var values = response.Headers.Location.Query.TrimStart('?').Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0], p => p[1]);

            Assert.True(values.ContainsKey(Constants.OAuth.CodeName));

            TokenRequestBuilder tokenBuilder = new TokenRequestBuilder()
                                                        .WithGrantType(Constants.OAuth.GrantType.AuthorizationCode)
                                                        .WithCode(values[Constants.OAuth.CodeName])
                                                        .WithClientId(TestConstants.Clients.DefaultCodeFlowClientId)
                                                        .WithClientSecret(TestConstants.Clients.DefaultCodeFlowClientSecret);

            var codeBody = JsonConvert.SerializeObject(tokenBuilder.Build());
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(codeBody);
            var content = new FormUrlEncodedContent(data);

            var uriBuilder = new UriBuilder(TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("openid10/token");
            var codeResponse = await this.TestClient.PostAsync(uriBuilder.ToString(), content);

            var deserialized = await codeResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            Assert.True(deserialized.ContainsKey(Constants.OAuth.ScopeName));

            var scopes = deserialized[Constants.OAuth.ScopeName].ToString().Split(" ");
            Assert.False(scopes.Contains(Constants.Scopes.ExternalToken.All));
            Assert.False(scopes.Contains(Constants.Scopes.ExternalToken.IdToken));
            Assert.False(scopes.Contains(Constants.Scopes.ExternalToken.AccessToken));

        }

        private class DummyLoginData : IExternalLoginData
        {
            private readonly string _sid;

            public DummyLoginData(string sid)
            {
                _sid = sid;
            }

            public ILoginRegistration LoginRegistration => new DummyWindowsLoginRegistration();

            public ClaimsIdentity Identity => null;

            public string InvitationCode => null;

            public Task<string> GetExternalIdentifierAsync()
            {
                return Task.FromResult(_sid);
            }
        }
    }
}
