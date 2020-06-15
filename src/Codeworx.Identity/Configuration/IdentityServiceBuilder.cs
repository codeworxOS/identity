using System.Reflection;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Login;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.OpenId.Authorization;
using Codeworx.Identity.View;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public class IdentityServiceBuilder : IIdentityServiceBuilder
    {
        public IdentityServiceBuilder(IServiceCollection collection)
        {
            ServiceCollection = collection;

            this.ReplaceService<IContentTypeLookup, ContentTypeLookup>(ServiceLifetime.Singleton);
            this.ReplaceService<IContentTypeProvider, DefaultContentTypeProvider>(ServiceLifetime.Singleton);

            this.AddAssets(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            this.ServiceCollection.AddSingleton<DefaultViewTemplate>();

            this.ReplaceService<ILoginViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<ITenantViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IFormPostResponseTypeTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());

            this.ReplaceService<ILoginViewService, LoginViewService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITenantViewService, TenantViewService>(ServiceLifetime.Scoped);
            this.ReplaceService<IExternalLoginService, ExternalLoginService>(ServiceLifetime.Scoped);
            this.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);

            ServiceCollection.AddScoped<IAuthorizationService, AuthorizationService>();
            ServiceCollection.AddScoped<IAuthorizationResponseProcessor, AccessTokenResponseProcessor>();
            ServiceCollection.AddScoped<IAuthorizationResponseProcessor, AuthorizationCodeResponseProcessor>();
            ServiceCollection.AddScoped<IAuthorizationResponseProcessor, IdTokenResponseProcessor>();

            ServiceCollection.AddScoped<ITokenService<TokenRequest>, TokenService>();
            ServiceCollection.AddScoped<ITokenService<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenService>();
            ServiceCollection.AddScoped<ITokenService<ClientCredentialsTokenRequest>, ClientCredentialsTokenService>();
            ServiceCollection.AddScoped<ITokenServiceSelector, TokenServiceSelector<AuthorizationCodeTokenRequest>>();
            ServiceCollection.AddScoped<ITokenServiceSelector, TokenServiceSelector<ClientCredentialsTokenRequest>>();

            ServiceCollection.AddTransient<IRequestValidator<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenRequestValidator>();
            ServiceCollection.AddTransient<IRequestValidator<ClientCredentialsTokenRequest>, ClientCredentialsTokenRequestValidator>();
        }

        public IServiceCollection ServiceCollection { get; }
    }
}