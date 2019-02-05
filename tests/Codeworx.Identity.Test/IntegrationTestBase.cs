using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

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
