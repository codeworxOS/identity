using System.IO;
using System.Reflection;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Test.Provider;
using Codeworx.Identity.Web.Test.Tenant;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptions<Configuration.IdentityOptions> identityOptions)
        {


            var supportedCultures = new[] { "en", "de" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCodeworxIdentity();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("bla/background.jpg", null);
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Path.GetTempPath(), "CodeworxIdentity.db")
            };

            services.AddCors(setup => setup.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                ));

            services.AddDbContext<DemoContext>((sp, builder) =>
            {
                var tenant = sp.GetService<IHttpContextAccessor>()?.HttpContext?.User?.FindFirst("current_tenant")?.Value;

                var connectionStringBuilder = new SqliteConnectionStringBuilder
                {
                    DataSource = Path.Combine(Path.GetTempPath(), $"{tenant ?? "default"}.db")
                };

                builder.UseSqlite(connectionStringBuilder.ConnectionString);
            });

            services.AddRouting();
            services.AddControllers();

            services.Configure<SmtpOptions>(this._configuration.GetSection("Smtp"));

            services.AddCodeworxIdentity()
                    //.Pbkdf2()
                    //.ReplaceService<IDefaultSigningKeyProvider, RsaDefaultSigningKeyProvider>(ServiceLifetime.Singleton)
                    //.ReplaceService<IScopeService, SampleScopeService>(ServiceLifetime.Singleton)
                    .AddAssets(Assembly.Load("Codeworx.Identity.Test.Theme"))
                    .AddSmtpMailConnector()
                    .AddMfaTotp()
                    .WithLoginAsEmail()
                    .UseDbContext(options => options.UseSqlite(connectionStringBuilder.ToString(), p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite")));
            //.UseDbContext(options => options.UseSqlServer("Data Source=.;Initial Catalog=IdentityTest; Integrated Security=True;", p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer")));
            //.UseConfiguration(_configuration);

            ////services.AddScoped<IClaimsService, SampleClaimsProvider>();

            services.AddAuthentication()
                .AddOAuth2Introspection("JWT", ConfigureIntrospection);
            //.AddNegotiate("Windows", p => { })
            //.AddJwtBearer("JWT", ConfigureJwt);


            services.AddAuthorization();

            services.AddMvcCore()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                });
        }

        private void ConfigureIntrospection(OAuth2IntrospectionOptions options)
        {
            options.Authority = "https://localhost:44319/";
            options.ClientCredentialStyle = IdentityModel.Client.ClientCredentialStyle.AuthorizationHeader;

            options.ClientId = TestConstants.Clients.DefaultBackendClientId;
            options.ClientSecret = TestConstants.Clients.DefaultBackendClientSecret;
        }

        private void ConfigureJwt(JwtBearerOptions options)
        {
            options.Authority = "https://localhost:44319/";
            options.Audience = "b45aba81aac1403f93dd1ce42f745ed2";
        }
    }
}