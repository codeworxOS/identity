using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.Test
{
    public abstract class IntegrationTestBase<TStartup> : IDisposable where TStartup : class
    {
        protected TestServer TestServer { get; }

        public HttpClient TestClient { get; }

        protected IntegrationTestBase()
        {
            var builder = new WebHostBuilder()
                .UseStartup<TStartup>();

            this.TestServer = new TestServer(builder);
            this.TestClient = this.TestServer.CreateClient();
        }

        protected async Task Authenticate()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var response = await this.TestClient.PostAsync(options.Value.AccountEndpoint + "/login",
                                                           new FormUrlEncodedContent(new Dictionary<string, string>
                                                                                     {
                                                                                         {"username", Constants.DefaultAdminUserName},
                                                                                         {"password", "admin"}
                                                                                     }));

            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            var authenticationCookie = cookies?.FirstOrDefault(p => p.StartsWith("identity"));
            this.TestClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, new[] { authenticationCookie });
        }

        public void Dispose()
        {
            this.TestServer?.Dispose();
            this.TestClient?.Dispose();
        }
    }

    public abstract class IntegrationTestBase : IntegrationTestBase<DefaultStartup>
    {

    }
}
