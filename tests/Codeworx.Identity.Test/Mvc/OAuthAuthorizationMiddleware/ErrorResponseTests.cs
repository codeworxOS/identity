using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.Mvc.OAuthAuthorizationMiddlewareTests
{
    public class ErrorResponseTests : IDisposable
    {
        private const string RequestPath = "/oauth20";

        private readonly HttpClient _testClient;
        private readonly TestServer _testServer;

        public ErrorResponseTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();

            _testServer = new TestServer(builder);
            _testClient = _testServer.CreateClient();
        }

        public void Dispose()
        {
            _testServer?.Dispose();
            _testClient?.Dispose();
        }

        [Fact]
        public async Task InvalidRequest_When_Query_Empty_Test()
        {
            var response = await _testClient.GetAsync(RequestPath);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains("Invalid request", responseHtml);
        }

        private class Startup
        {
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<IdentityOptions> options)
            {
                app.UseCodeworxIdentity(options.Value);
            }

            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCodeworxIdentity();

                //var builder = new IdentityServiceBuilder(services, Constants.DefaultAuthenticationScheme);

                //services.AddTransient<IContentTypeProvider, DefaultContentTypeProvider>();
                //services.AddSingleton(p => builder.ToService(p.GetService<IOptions<IdentityOptions>>().Value, p.GetService<IEnumerable<IContentTypeProvider>>()));

                //services.AddAuthentication(options => options.DefaultAuthenticateScheme = Constants.DefaultAuthenticationScheme)
                //        .AddCookie(Constants.DefaultAuthenticationScheme);
            }
        }
    }
}