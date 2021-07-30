using System.IO;
using System.Reflection;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.Web.Test.Tenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<Configuration.IdentityOptions> identityOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCodeworxIdentity(identityOptions.Value);
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
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

            services.AddCodeworxIdentity(_configuration)
                    //.Pbkdf2()
                    //.ReplaceService<IDefaultSigningKeyProvider, RsaDefaultSigningKeyProvider>(ServiceLifetime.Singleton)
                    //.ReplaceService<IScopeService, SampleScopeService>(ServiceLifetime.Singleton)
                    .AddAssets(Assembly.Load("Codeworx.Identity.Test.Theme"))
                    .UseDbContext(options => options.UseSqlite(connectionStringBuilder.ToString(), p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite")));
                    //.UseDbContext(options => options.UseSqlServer("Data Source=.;Initial Catalog=IdentityTest; Integrated Security=True;", p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer")));
            //.UseConfiguration(_configuration);

            ////services.AddScoped<IClaimsService, SampleClaimsProvider>();

            services.AddAuthentication()
                //.AddNegotiate("Windows", p => { })
                .AddJwtBearer("JWT", ConfigureJwt);

            services.AddAuthorization();

            services.AddMvcCore()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                });
        }

        private void ConfigureJwt(JwtBearerOptions options)
        {
            options.Authority = "https://localhost:44319/";
            options.Audience = "B45ABA81-AAC1-403F-93DD-1CE42F745ED2";
        }
    }
}