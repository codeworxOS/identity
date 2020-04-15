using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;
using Codeworx.Identity.Cryptography;

namespace Codeworx.Identity.Web.Test
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<Configuration.IdentityOptions> identityOptions)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCodeworxIdentity(identityOptions.Value);
            app.UseMvc();
            app.UseStaticFiles();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Path.GetTempPath(), "CodeworxIdentity.db")
            };

            services.AddSingleton<IExternalLoginProvider, WindowsLoginProvider>();

            services.AddCodeworxIdentity(_configuration)
                    //.ReplaceService<IDefaultSigningKeyProvider, RsaDefaultSigningKeyProvider>(ServiceLifetime.Singleton)
                    .AddAssets(Assembly.Load("Codeworx.Identity.Test.Theme"))
                    .UseTestSetup()
                    .UseDbContext(options => options.UseSqlite(connectionStringBuilder.ToString()))
                    .UseConfiguration(_configuration);

            services.AddScoped<IClaimsService, SampleClaimsProvider>();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }
    }
}