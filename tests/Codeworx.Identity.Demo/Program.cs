using Codeworx.Identity.Configuration;
using Codeworx.Identity.Demo.Database;
using Codeworx.Identity.EntityFrameworkCore.Api;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Codeworx.Identity.Mail;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

internal class Program
{
    private static void ConfigureIntrospect(OAuth2IntrospectionOptions options)
    {
        options.ClientId = "18d1fb80b3974e78be9e01c90e20d5f0";
        options.ClientSecret = "clientsecret";
        options.ClientCredentialStyle = IdentityModel.Client.ClientCredentialStyle.AuthorizationHeader;
        options.Authority = "https://localhost:7127/";

        options.CacheDuration = TimeSpan.FromMinutes(30);
    }

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(Path.GetTempPath(), "IdentityDemo.db"),
        };

        // setup scim
        builder.Services.AddScimEndpoint<DemoIdentityDbContext>();
        builder.Services.AddScimSchema<UserResource>(ScimConstants.ResourceTypes.User, ScimConstants.Schemas.User);
        builder.Services.AddScimSchema<GroupResource>(ScimConstants.ResourceTypes.Group, ScimConstants.Schemas.Group);
        builder.Services.AddScimSchema<EnterpriseUserResource>(ScimConstants.ResourceTypes.EnterpriseUser, ScimConstants.Schemas.EnterpriseUser);

        builder.Services.AddScimProperties<User, UserResource>(d => d
                                                    .AddClrProperty(p => p.Active, d => !d.IsDisabled, (d, v) => d.IsDisabled = !v.GetValueOrDefault(true))
                                                    .AddClrProperty(d => d.UserName, d => d.Name)
                                                    .AddShadowProperty(p => p.Name!.GivenName, "FirstName")
                                                    .AddShadowProperty(p => p.Name!.FamilyName, "LastName")
                                                    .AddShadowProperty<EmailResource, string>(p => p.Emails!, "Email", "work", true));

        builder.Services.AddScimProperties<User, ScimResponseInfo>(d => d.AddClrProperty(d => d.Id, d => d.Id.ToString("N"), true)
                                                                 .AddClrProperty(d => d.Created, d => d.Created, true)
                                                                 .AddClrProperty(d => d.LastModified, d => d.Created, true));

        builder.Services.AddScimProperties<Group, GroupResource>(d => d
                                                    .AddClrProperty(d => d.DisplayName, d => d.Name)
                                                    .AddNavigationProperty(
                                                                d => d.Members!,
                                                                d => d.Members.Select(x => new GroupMemberResource
                                                                {
                                                                    Value = x.RightHolderId.ToString("N"),
                                                                    Ref = x.RightHolder is User ? "user" : "group",
                                                                    Display = x.RightHolder.Name,
                                                                }).ToList()));

        builder.Services.AddScimProperties<Group, ScimResponseInfo>(d => d.AddClrProperty(d => d.Id, d => d.Id.ToString("N"), true));

        ////builder.Services.AddScimProperties<User, EnterpriseUserResource>(d => d.Schema(ScimConstants.Schemas.EnterpriseUser));

        // setup swagger
        builder.Services.AddScoped<Codeworx.Identity.EntityFrameworkCore.Api.IContextWrapper, Codeworx.Identity.EntityFrameworkCore.Api.DbContextWrapper<DemoIdentityDbContext>>();
        builder.Services.AddScoped<Codeworx.Identity.EntityFrameworkCore.Scim.Api.IContextWrapper, Codeworx.Identity.EntityFrameworkCore.Scim.Api.DbContextWrapper<DemoIdentityDbContext>>();
        builder.Services.AddOpenApiDocument<DemoIdentityDbContext>((options, s) =>
        {
            options.Title = "Codeworx Identity Demo";

            options.AddSecurity("oauth2", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = $"https://localhost:7127/openid10",
                        TokenUrl = $"https://localhost:7127/openid10/token",
                        Scopes = new Dictionary<string, string> { { "openid", "Api Access" } },
                    },
                },
            });

            options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
        });

        // Setup MVC Middleware and add identity admin controllers
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(UserController).Assembly)
            .AddApplicationPart(typeof(Codeworx.Identity.EntityFrameworkCore.Scim.Api.UsersController).Assembly);

        // setup authentication providers
        builder.Services.AddAuthentication()
           .AddNegotiate("Windows", p => { }) // remove if using iisexpress or iis
           .AddOAuth2Introspection("JWT", ConfigureIntrospect);

        // setup authorziation policies
        builder.Services.AddAuthorization(p =>
        {
            p.FallbackPolicy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .AddAuthenticationSchemes("JWT")
                                    .Build();

            // policy for admin controllers
            p.AddPolicy(Policies.Admin, builder => builder.RequireAuthenticatedUser()
                                            .RequireClaim("upn", "admin")
                                            .AddAuthenticationSchemes("JWT"));
        });

        // setup custom identity DbContext.
        builder.Services.AddDbContext<DemoIdentityDbContext>(p =>
            p.UseSqlite(connectionStringBuilder.ConnectionString)
            .UseDataMigrations());

        // configure smtp connector form appsettings
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

        // configure identity options from appsettings
        builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection("Identity"));

        // setup identity
        builder.Services.AddCodeworxIdentity()
            .AddSmtpMailConnector() // smtp settings in appsettings.json
            .AddMfaTotp() // add Totp MFA Provider
            .ReplaceService<IMailAddressProvider, MailPropertyAddressProvider>(ServiceLifetime.Scoped) // user e-mail from Email shadow property
            /*.ReplaceService<IPasswordChangeViewTemplate, MyPasswordChangeViewTemplate>(ServiceLifetime.Singleton)
            .RegisterMultiple<IPartialTemplate, MyFormsLoginTemplate>(ServiceLifetime.Singleton)
            .ReplaceService<IStringResources, MyStringResources>(ServiceLifetime.Singleton)
            .UseDbContextSqlite(connectionStringBuilder.ConnectionString); */
            .UseDbContext<DemoIdentityDbContext>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseRequestLocalization("de", "en");
        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseSwaggerUi(options =>
        {
            options.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = "52b0bd4db6a74b4e9879211b0dc61744",
                AppName = "Identity-Demo",
                UsePkceWithAuthorizationCodeGrant = true,
            };
        });

        app.UseOpenApi();

        // add identity server middleware
        app.UseCodeworxIdentity();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        // Migrate demo db
        await app.Services.MigrateDatabaseAsync<DemoIdentityDbContext>();
        await app.RunAsync();
    }
}