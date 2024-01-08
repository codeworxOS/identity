using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Api;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Test.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Codeworx.Identity.Api.Test
{
    public class Startup
    {
        private static readonly string ClientId = "809b3854c35449b990dc83f80ac5f4c2";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SmtpOptions>(Configuration.GetSection("Smtp"));

            IMvcBuilder mvcBuilder = services
                .AddControllers()
                .AddApplicationPart(typeof(TenantController).Assembly)
                .AddNewtonsoftJson();

            ////services.AddOpenApiDocument((document, sp) =>
            services.AddOpenApiDocument<TestIdentityContext>((document, sp) =>
            {
                document.AddSecurity("JWT", new string[] { }, new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "My Authentication",
                    Flow = OpenApiOAuth2Flow.Implicit,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            Scopes = { { Constants.OpenId.Scopes.OpenId, "OpenId scope" } },
                            TokenUrl = "https://localhost:44371/oauth20/token",
                            AuthorizationUrl = "https://localhost:44371/oauth20",
                        },
                    },
                });

                document.PostProcess = doc =>
                {
                    var test = doc.OpenApi;
                };

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddScoped<IContextWrapper, DbContextWrapper<TestIdentityContext>>();

            services.AddDbContext<TestIdentityContext>(p => p.UseSqlite("Data Source=apitest.sqlite"));

            services.AddAuthorization(p =>
            {
                p.AddPolicy(Policies.Admin, builder => builder.AddAuthenticationSchemes("JWT").RequireClaim("upn", "admin"));
            });

            services.AddTransient<IAuthorizationHandler, DebugAuthorizationHandler>();

            services.AddCodeworxIdentity()
                .UseDbContext<TestIdentityContext>()
                .WithLoginAsEmail()
                .AddSmtpMailConnector();

            services.AddAuthentication()
                .AddJwtBearer("JWT", ConfigureJwt);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<IdentityOptions> identityOptions, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<TestIdentityContext>();
                if (ctx.Database.EnsureCreatedAsync().Result)
                {
                    var hashing = scope.ServiceProvider.GetRequiredService<IHashingProvider>();

                    var user = new EntityFrameworkCore.Model.User
                    {
                        Id = Guid.NewGuid(),
                        Name = "raphael@codeworx.at",
                    };

                    ctx.Users.Add(user);

                    var entry = ctx.Entry(user);
                    entry.CurrentValues["FirstName"] = "Raphael";
                    entry.CurrentValues["LastName"] = "Schwarz";

                    var admin = new EntityFrameworkCore.Model.User
                    {
                        Id = Guid.NewGuid(),
                        Name = "admin",
                        PasswordHash = hashing.Create("admin"),
                    };

                    ctx.Users.Add(admin);

                    var client = new EntityFrameworkCore.Model.ClientConfiguration
                    {
                        Id = Guid.Parse(ClientId),
                        ClientType = Model.ClientType.UserAgent,
                        TokenExpiration = TimeSpan.FromHours(1),
                        ValidRedirectUrls =
                        {
                            new EntityFrameworkCore.Model.ValidRedirectUrl
                            {
                                 Id = Guid.NewGuid(),
                                 Url = "/swagger/oauth2-redirect.html",
                            },
                        },
                    };

                    ctx.ClientConfigurations.Add(client);

                    var forms = new EntityFrameworkCore.Model.AuthenticationProvider
                    {
                        Id = Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id),
                        Name = TestConstants.LoginProviders.FormsLoginProvider.Name,
                        EndpointType = "forms",
                        SortOrder = 0,
                    };
                    ctx.AuthenticationProviders.Add(forms);

                    ctx.SaveChanges();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCodeworxIdentity();

            app.UseSwaggerUi(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = ClientId,
                };
            });
            app.UseOpenApi();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureJwt(JwtBearerOptions options)
        {
            options.Authority = "https://localhost:44371";
            options.Audience = "809b3854c35449b990dc83f80ac5f4c2";
            options.MapInboundClaims = false;
        }

        private class DebugAuthorizationHandler : IAuthorizationHandler
        {
            public Task HandleAsync(AuthorizationHandlerContext context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
