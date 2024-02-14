using System.Security.Claims;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Test.Provider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Test
{
    public class DefaultStartup
    {
        private readonly IConfiguration _configuration;

        public DefaultStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IOptions<IdentityOptions> options, IAuthenticationSchemeProvider schemeProvider)
        {
            app.UseCodeworxIdentity();

            app.Map(
                "/test-setup/windowslogin",
                p => p.Use((next) => async (ctx) =>
                {
                    var identity = new ClaimsIdentity(Constants.WindowsAuthenticationSchema);
                    identity.AddClaim(new Claim(Constants.Claims.Upn, ctx.Request.Form["username"]));
                    identity.AddClaim(new Claim(ClaimTypes.PrimarySid, ctx.Request.Form["sid"]));
                    var principal = new ClaimsPrincipal(identity);

                    await ctx.SignInAsync(Constants.WindowsAuthenticationSchema, principal);
                }));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddCookie(Constants.WindowsAuthenticationSchema);

            var builder = services
                .AddCodeworxIdentity()
                .WithLoginAsEmail()
                .AddMfaTotp()
                .UseTestSetup()
                .LoginRegistrations<DummyLoginRegistrationProviderWithTotp>();
        }
    }
}