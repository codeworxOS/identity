using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Test
{
    public class DefaultStartup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<IdentityOptions> options, IAuthenticationSchemeProvider schemeProvider)
        {
            app.UseCodeworxIdentity(options.Value);

            schemeProvider.RemoveScheme(Constants.DefaultAuthenticationScheme);
            schemeProvider.AddScheme(new AuthenticationScheme(Constants.DefaultAuthenticationScheme, "Test Scheme", typeof(TestAuthenticationHandler)));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeworxIdentity();
        }
    }

    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
                                                                             {
                                                                                 new Claim(Constants.NameClaimType, "UnitTest User"),
                                                                                 new Claim(Constants.IdClaimType, Constants.DefaultAdminUserId),
                                                                             },
                                                                             "test");
    }

    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(Options.Identity),
                                                                new AuthenticationProperties(),
                                                                Constants.DefaultAuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }
}