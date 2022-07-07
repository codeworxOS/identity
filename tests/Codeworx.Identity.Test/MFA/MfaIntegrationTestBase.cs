using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OtpNet;
using static Codeworx.Identity.Test.DummyUserService;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaIntegrationTestBase : IntegrationTestBase
    {
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

            var scopes = "openid offline_access tenant";
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


        protected async Task<HttpResponseMessage> SelectTenant(HttpResponseMessage authorizationResponse, string tenant)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var isRedirectToTenantSelection = authorizationResponse.StatusCode == HttpStatusCode.Redirect
                && authorizationResponse.Headers.Location.ToString().Contains(options.Value.SelectTenantEndpoint);
            if (!isRedirectToTenantSelection)
            {
                throw new ArgumentException("expected a redirect to tenant selection page");
            }

            var tenantSelectionUrl = authorizationResponse.Headers.Location.ToString();
            var selectTenantResponse = await this.TestClient.PostAsync(tenantSelectionUrl, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"tenantKey", tenant},
                {"setDefault", false.ToString() }

            }));

            return selectTenantResponse;
        }

        protected async Task<HttpResponseMessage> FulfillMfa(string sharedSecret, HttpResponseMessage loginResponse = null)
        {
            if (loginResponse != null)
            {
                var isRedirectToMfa = loginResponse.StatusCode == HttpStatusCode.Redirect
                && loginResponse.Headers.Location.ToString().Contains("login/mfa");
                if (!isRedirectToMfa) { 
                    throw new ArgumentException("expected a redirect to mfa page");
                }
            }

            var mfaUrl = loginResponse?.Headers.Location.ToString() ?? GetMfaUrl().ToString();
            var providerId = Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id).ToString("N");

            var key = Base32Encoding.ToBytes(sharedSecret);
            var otpProvider = new Totp(key);
            var oneTimeCode = otpProvider.ComputeTotp(DateTime.Now);

            var response = await this.TestClient.PostAsync(mfaUrl,
                   new FormUrlEncodedContent(new Dictionary<string, string>
                   {
                       {"provider-id", providerId },
                       {"one-time-code", oneTimeCode}
                   }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);
            var mfaCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity.mfa")); // TODO change cookie name
            if (!string.IsNullOrEmpty(mfaCookie))
            {
                this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { mfaCookie });
            }

            return response;
        }

        protected async Task<HttpResponseMessage> GetToken(HttpResponseMessage authorizationResponse)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var tokenRequestBuilder = new TokenRequestBuilder()
                .WithGrantType(Constants.OAuth.GrantType.AuthorizationCode)
                .WithRedirectUri(this.GetRedirectUrl().ToString());

            var authorizationRequestQueryParameters = this.GetQueryParameters(authorizationResponse.RequestMessage.RequestUri.Query);
            var clientId = authorizationRequestQueryParameters[Constants.OAuth.ClientIdName];
            tokenRequestBuilder.WithClientId(clientId);

            var redirectQueryParameters = this.GetQueryParameters(authorizationResponse.Headers.Location.Query);
            var code = redirectQueryParameters[Constants.OAuth.CodeName];
            tokenRequestBuilder.WithCode(code);
                
            var tokenRequest = tokenRequestBuilder.Build();
            var tokenResponse = await this.TestClient.PostAsync(options.Value.OpenIdTokenEndpoint, this.GetRequestBody(tokenRequest));

            return tokenResponse;
        }

        protected async Task<TokenResponse> ExtractToken(HttpResponseMessage tokenResponse)
        {
            var tokenResponseData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
            return tokenResponseData;
        }

        protected async Task<HttpResponseMessage> RefreshToken(TokenResponse token)
        {
            if (string.IsNullOrEmpty(token.RefreshToken))
            {
                throw new ArgumentException("no refresh token available");
            }

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var tokenRequestBuilder = new TokenRequestBuilder()
                .WithGrantType(Constants.OAuth.GrantType.RefreshToken)
                .WithRefreshCode(token.RefreshToken);

            var jwtToken = new JwtSecurityToken(token.AccessToken);
            var clientId = jwtToken.Audiences.First();
            tokenRequestBuilder.WithClientId(clientId);

            var tokenRequest = tokenRequestBuilder.Build();

            var refreshTokenResponse = await this.TestClient.PostAsync(options.Value.OpenIdTokenEndpoint, this.GetRequestBody(tokenRequest));

            return refreshTokenResponse;
        }

        protected async Task<HttpResponseMessage> GetOnBehalfOfToken(string sourceClientId, string targetClientId, TokenResponse sourceToken)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var tokenRequest = new TokenRequestBuilder()
                .WithGrantType(Constants.OAuth.GrantType.TokenExchange)
                .WithClientId(sourceClientId)
                .WithAudience(targetClientId)
                .WithSubjectToken(sourceToken.AccessToken)
                .WithRequestedTokenType($"{Constants.TokenExchange.TokenType.AccessToken} {Constants.TokenExchange.TokenType.IdToken}")
                .WithScopes("openid")
                .Build();

            var tokenResponse = await this.TestClient.PostAsync(options.Value.OpenIdTokenEndpoint, this.GetRequestBody(tokenRequest));
            return tokenResponse;
        }

        protected async Task<HttpResponseMessage> GetTokenFromApiKey(string clientId, string clientSecret, string defaultTenant)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var tokenRequest = new TokenRequestBuilder()
                .WithGrantType(Constants.OAuth.GrantType.ClientCredentials)
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                .WithScopes($"openid tenant {defaultTenant}")
                .Build();

            var tokenResponse = await this.TestClient.PostAsync(options.Value.OpenIdTokenEndpoint, this.GetRequestBody(tokenRequest));
            return tokenResponse;
        }

        protected bool HasLoginCookie(HttpResponseMessage loginResponse)
        {
            if (loginResponse.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies))
            {
                var hasLoginCookie = cookies.Any(p => p.StartsWith("identity"));
                return hasLoginCookie;
            }

            return false;
        }

        protected bool HasMfaCookie(HttpResponseMessage loginResponse)
        {
            if (loginResponse.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies))
            {
                var hasMfaCookie = cookies.Any(p => p.StartsWith("identity.mfa")); // TODO change cookie name
                return hasMfaCookie;
            }

            return false;
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

        private FormUrlEncodedContent GetRequestBody(TokenRequest tokenRequest)
        {
            var body = JsonConvert.SerializeObject(tokenRequest);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var content = new FormUrlEncodedContent(data);
            return content;
        }

        private Dictionary<string, string> GetQueryParameters(string queryString)
        {
            var queryParameters = queryString.TrimStart('?').Split("&").ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);
            return queryParameters;
        }
    }
}
