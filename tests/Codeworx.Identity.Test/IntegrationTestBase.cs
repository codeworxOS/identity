using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Test.Provider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace Codeworx.Identity.Test
{
    public abstract class IntegrationTestBase<TStartup> where TStartup : class
    {
        protected TestServer TestServer { get; private set; }

        public HttpClient TestClient { get; private set; }

        [SetUp()]
        public void Setup()
        {
            var builder = new WebHostBuilder()
                .UseStartup<TStartup>();

            this.TestServer = new TestServer(builder);
            this.TestClient = this.TestServer.CreateClient();
        }

        protected virtual async Task<HttpResponseMessage> Authenticate(string userName, string password)
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var loginRequestBuilder = new UriBuilder(this.TestClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.Value.AccountEndpoint);
            loginRequestBuilder.AppendPath("login");
            ////loginRequestBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, this.GetRedirectUrl().ToString());

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

        protected virtual async Task Authenticate()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var response = await this.TestClient.PostAsync(options.Value.AccountEndpoint + "/login",
                                                           new FormUrlEncodedContent(new Dictionary<string, string>
                                                                                     {
                                                                                         {"provider-id", TestConstants.LoginProviders.FormsLoginProvider.Id},
                                                                                         {"username", TestConstants.Users.DefaultAdmin.UserName},
                                                                                         {"password", TestConstants.Users.DefaultAdmin.Password}
                                                                                     }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });
        }

        [TearDown()]
        public void TearDown()
        {
            this.TestServer?.Dispose();
            this.TestClient?.Dispose();
        }
    }

    public abstract class IntegrationTestBase : IntegrationTestBase<DefaultStartup>
    {

    }
}
