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
using OtpNet;
using static Codeworx.Identity.Test.DummyUserService;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaIntegrationTestBase : IntegrationTestBase
    {
        private const string MfaSharedSecret = "HXGWF5E326N665KU";

        protected void ConfigureMfaTestUser(bool isMfaRequired, bool isMfaConfigured)
        {
            var dummyUserService = (DummyUserService)this.TestServer.Host.Services.GetRequiredService<IUserService>();
            var mfaTestUser = (MfaTestUser)dummyUserService.Users.FirstOrDefault(user => Guid.Parse(user.Identity) == Guid.Parse(TestConstants.Users.MfaTestUser.UserId));

            mfaTestUser.SetMfaRequired(isMfaRequired);
            if (isMfaConfigured)
            {
                mfaTestUser.RegisterMfa(Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id).ToString("N"), MfaSharedSecret);
            }
        }

        protected async Task<HttpResponseMessage> Authenticate(string userName, string password)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.Value.AccountEndpoint);
            loginRequestBuilder.AppendPath("login");
            loginRequestBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, this.GetRedirectUrl().ToString());

            var response = await this.TestClient.PostAsync(loginRequestBuilder.ToString(),
                               new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                   {"username", userName},
                                   {"password", password}
                               }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);
            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            if (!string.IsNullOrEmpty(authenticationCookie)) 
            { 
                this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });
            }

            return response;
        }

        protected async Task<HttpResponseMessage> GetAuthorizationResponse(string clientId, string defaultTenant)
        {
            var authorizationRequestBuilder = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(clientId);

            var scopes = "openid tenant";
            if (!string.IsNullOrEmpty(defaultTenant))
            {
                scopes += $" {defaultTenant}";
            } 
            authorizationRequestBuilder.WithScope(scopes);
            
            var authorizationRequest = authorizationRequestBuilder.Build();

            var uriBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            uriBuilder.AppendPath("openid10");
            authorizationRequest.Append(uriBuilder);
            var authorizationResponse = await this.TestClient.GetAsync(uriBuilder.ToString());
            return authorizationResponse;
        }

        protected async Task<HttpResponseMessage> FulfillMfa(HttpResponseMessage authorizationResponse = null)
        {
            if (authorizationResponse != null)
            {
                var isRedirectToMfa = authorizationResponse.StatusCode == HttpStatusCode.Redirect
                && authorizationResponse.Headers.Location.ToString().Contains("login/mfa");
                if (!isRedirectToMfa) { 
                    throw new ArgumentException("expected a redirect to mfa page");
                }
            }

            var mfaUrl = authorizationResponse?.Headers.Location.ToString() ?? GetMfaUrl().ToString();
            var providerId = Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id).ToString("N");

            var key = Base32Encoding.ToBytes(MfaSharedSecret);
            var otpProvider = new Totp(key);
            var oneTimeCode = otpProvider.ComputeTotp(DateTime.Now);

            var response = await this.TestClient.PostAsync(mfaUrl,
                   new FormUrlEncodedContent(new Dictionary<string, string>
                   {
                       {"provider-id", providerId },
                       {"one-time-code", oneTimeCode}
                   }));

            return response;
        }

        protected Uri GetRedirectUrl()
        {
            return new Uri("https://example.org/redirect");
        }

        protected Uri GetMfaUrl()
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
