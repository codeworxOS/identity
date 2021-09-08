using System.Reflection;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Login.Windows;
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
            this.ReplaceService<DefaultViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton);
            this.ReplaceService<DefaultViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton);

            this.ReplaceService<ILoginViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<ITenantViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IFormPostResponseTypeTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IRedirectViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IInvitationViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IPasswordChangeViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            this.ReplaceService<IProfileViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());

            this.ReplaceService<ILoginViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<ITenantViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<IFormPostResponseTypeTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<IRedirectViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<IInvitationViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<IPasswordChangeViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            this.ReplaceService<IProfileViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());

            this.ReplaceService<IInvitationService, InvitationService>(ServiceLifetime.Scoped);
            this.ReplaceService<IInvitationViewService, InvitationViewService>(ServiceLifetime.Scoped);

            this.ReplaceService<IProfileService, ProfileService>(ServiceLifetime.Scoped);

            this.ReplaceService<ILoginViewService, LoginViewService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITenantViewService, TenantViewService>(ServiceLifetime.Scoped);
            this.ReplaceService<ILoginService, LoginService>(ServiceLifetime.Scoped);
            this.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);
            this.ReplaceService<IPasswordPolicyProvider, PasswordPolicyProvider>(ServiceLifetime.Scoped);
            this.ReplaceService<IPasswordChangeService, PasswordChangeService>(ServiceLifetime.Scoped);

            this.ReplaceService<WindowsLoginProcessor, WindowsLoginProcessor>(ServiceLifetime.Scoped);
            this.ReplaceService<OAuthLoginProcessor, OAuthLoginProcessor>(ServiceLifetime.Scoped);
            this.ReplaceService<FormsLoginProcessor, FormsLoginProcessor>(ServiceLifetime.Scoped);
            this.PasswordValidator<PasswordValidator>();

            this.ReplaceService<IOAuthLoginService, OAuthLoginService>(ServiceLifetime.Transient);
            this.RegisterMultiple<IProcessorTypeLookup, WindowsLoginProcessorLookup>(ServiceLifetime.Singleton);
            this.RegisterMultiple<IProcessorTypeLookup, ExternalOAuthLoginProcessorLookup>(ServiceLifetime.Singleton);
            this.RegisterMultiple<IProcessorTypeLookup, FormsLoginProcessorLookup>(ServiceLifetime.Singleton);

            this.ReplaceService(typeof(IAuthorizationService<>), typeof(AuthorizationService<>), ServiceLifetime.Scoped);
            this.RegisterMultiple<IAuthorizationResponseProcessor, AccessTokenResponseProcessor>(ServiceLifetime.Scoped);
            this.RegisterMultiple<IAuthorizationResponseProcessor, AuthorizationCodeResponseProcessor>(ServiceLifetime.Scoped);
            this.RegisterMultiple<IAuthorizationResponseProcessor, IdTokenResponseProcessor>(ServiceLifetime.Scoped);

            this.ReplaceService<ITokenService<TokenRequest>, TokenService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITokenService<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITokenService<ClientCredentialsTokenRequest>, ClientCredentialsTokenService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITokenService<RefreshTokenRequest>, RefreshTokenService>(ServiceLifetime.Scoped);

            this.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<AuthorizationCodeTokenRequest>>(ServiceLifetime.Scoped);
            this.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<ClientCredentialsTokenRequest>>(ServiceLifetime.Scoped);
            this.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<RefreshTokenRequest>>(ServiceLifetime.Scoped);

            this.ReplaceService<IRequestValidator<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenRequestValidator>(ServiceLifetime.Transient);

            this.RegisterMultiple<IPartialTemplate, FormsLoginTemplate>(ServiceLifetime.Transient);
            this.RegisterMultiple<IPartialTemplate, FormsProfileTemplate>(ServiceLifetime.Transient);
            this.RegisterMultiple<IPartialTemplate, RedirectLinkTemplate>(ServiceLifetime.Transient);
            this.RegisterMultiple<IPartialTemplate, RedirectLinkProfileTemplate>(ServiceLifetime.Transient);
            this.RegisterMultiple<IPartialTemplate, FormsInvitationTemplate>(ServiceLifetime.Transient);
        }

        public IServiceCollection ServiceCollection { get; }
    }
}