using System;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.EntityFrameworkCore.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Api.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();

            ////services.AddOpenApiDocument();
            services.AddOpenApiDocument<TestIdentityContext>();
            services.AddScoped<IContextWrapper, DbContextWrapper<TestIdentityContext>>();

            services.AddDbContext<TestIdentityContext>(p => p.UseSqlite("Data Source=apitest.sqlite"));

            services.AddCodeworxIdentity(this.Configuration)
                .UseDbContext<TestIdentityContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<IdentityOptions> identityOptions, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<TestIdentityContext>();
                if (ctx.Database.EnsureCreatedAsync().Result)
                {
                    var user = new EntityFrameworkCore.Model.User
                    {
                        Id = Guid.NewGuid(),
                        Name = "raphael@codeworx.at",
                    };

                    ctx.Users.Add(user);

                    var entry = ctx.Entry(user);
                    entry.CurrentValues["FirstName"] = "Raphael";
                    entry.CurrentValues["LastName"] = "Schwarz";

                    ctx.SaveChanges();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCodeworxIdentity(identityOptions.Value);

            app.UseSwaggerUi3();
            app.UseOpenApi();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
