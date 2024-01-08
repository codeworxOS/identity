using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Test
{
    public class UrlValidationStartup : DefaultStartup
    {
        public UrlValidationStartup(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddAuthentication().AddCookie(Constants.WindowsAuthenticationSchema, _ => { });

            var builder = new IdentityServiceBuilder(services);
            builder.AddSmtpMailConnector();
        }
    }
}