using System;
using System.Reflection;
using Codeworx.Identity;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Mfa;
using Codeworx.Identity.Mfa.Mail;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.OpenId.Authorization;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Token;
using Codeworx.Identity.Token.Reference;
using Codeworx.Identity.View;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder RegisterCoreServices(this IIdentityServiceBuilder builder)
        {
            builder.ReplaceService<IContentTypeLookup, ContentTypeLookup>(ServiceLifetime.Singleton);
            builder.ReplaceService<IContentTypeProvider, DefaultContentTypeProvider>(ServiceLifetime.Singleton);

            builder.AddAssets(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            builder.ReplaceService<DefaultViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton);
            builder.ReplaceService<DefaultViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton);

            builder.ReplaceService<ILoginViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<ITenantViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IFormPostResponseTypeTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IRedirectViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IInvitationViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IPasswordChangeViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IProfileViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IForgotPasswordViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());
            builder.ReplaceService<IConfirmationViewTemplate, DefaultViewTemplate>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplate>());

            builder.ReplaceService<ILoginViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<ITenantViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IFormPostResponseTypeTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IRedirectViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IInvitationViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IPasswordChangeViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IProfileViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IForgotPasswordViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());
            builder.ReplaceService<IConfirmationViewTemplateCache, DefaultViewTemplateCache>(ServiceLifetime.Singleton, sp => sp.GetRequiredService<DefaultViewTemplateCache>());

            builder.ReplaceService<IInvitationService, InvitationService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IInvitationViewService, InvitationViewService>(ServiceLifetime.Scoped);

            builder.ReplaceService<IProfileService, ProfileService>(ServiceLifetime.Scoped);

            builder.ReplaceService<ILoginViewService, LoginViewService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IMfaViewService, MfaViewService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITenantViewService, TenantViewService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ILoginService, LoginService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ILoginDelayService, DelayService>(ServiceLifetime.Singleton);
            builder.ReplaceService<IForgotPasswordDelayService, DelayService>(ServiceLifetime.Singleton);
            builder.ReplaceService<IPasswordPolicyProvider, DefaultPasswordPolicyProvider>(ServiceLifetime.Singleton);
            builder.ReplaceService<ILoginPolicyProvider, DefaultLoginPolicyProvider>(ServiceLifetime.Singleton);
            builder.ReplaceService<IPasswordChangeService, PasswordChangeService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IForgotPasswordService, ForgotPasswordService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IConfirmationViewService, ConfirmationViewService>(ServiceLifetime.Scoped);
            builder.ReplaceService<INotificationService, NotificationService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenProviderService, TokenProviderService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenProvider, ReferenceTokenProvider>(ServiceLifetime.Scoped);

            builder.ReplaceService<WindowsLoginProcessor, WindowsLoginProcessor>(ServiceLifetime.Scoped);
            builder.ReplaceService<OAuthLoginProcessor, OAuthLoginProcessor>(ServiceLifetime.Scoped);
            builder.ReplaceService<FormsLoginProcessor, FormsLoginProcessor>(ServiceLifetime.Scoped);
            builder.ReplaceService<MailMfaLoginProcessor, MailMfaLoginProcessor>(ServiceLifetime.Scoped);
            builder.PasswordValidator<PasswordValidator>();

            builder.ReplaceService<IOAuthLoginService, OAuthLoginService>(ServiceLifetime.Transient);
            builder.RegisterMultiple<IProcessorTypeLookup, MailMfaLoginProcessorLookup>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IProcessorTypeLookup, WindowsLoginProcessorLookup>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IProcessorTypeLookup, ExternalOAuthLoginProcessorLookup>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IProcessorTypeLookup, FormsLoginProcessorLookup>(ServiceLifetime.Singleton);

            builder.ReplaceService(typeof(IAuthorizationService<>), typeof(AuthorizationService<>), ServiceLifetime.Scoped);
            builder.RegisterMultiple<IAuthorizationResponseProcessor, AccessTokenResponseProcessor>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<IAuthorizationResponseProcessor, AuthorizationCodeResponseProcessor>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<IAuthorizationResponseProcessor, IdTokenResponseProcessor>(ServiceLifetime.Scoped);

            builder.ReplaceService<ITokenService<TokenRequest>, TokenService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenService<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenService<ClientCredentialsTokenRequest>, ClientCredentialsTokenService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenService<RefreshTokenRequest>, RefreshTokenService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITokenService<TokenExchangeRequest>, TokenExchangeService>(ServiceLifetime.Scoped);

            builder.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<AuthorizationCodeTokenRequest>>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<ClientCredentialsTokenRequest>>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<RefreshTokenRequest>>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<ITokenServiceSelector, TokenServiceSelector<TokenExchangeRequest>>(ServiceLifetime.Scoped);

            builder.ReplaceService<IRequestValidator<AuthorizationCodeTokenRequest>, AuthorizationCodeTokenRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<MfaLoginRequest>, MfaLoginRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<WindowsLoginRequest>, WindowsLoginRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<OAuthRedirectRequest>, OAuthRedirectRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<PasswordChangeRequest>, PasswordChangeRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<LogoutRequest>, LogoutRequestValidator>(ServiceLifetime.Transient);
            builder.ReplaceService<IRequestValidator<LoginRequest>, LoginRequestValidator>(ServiceLifetime.Transient);

            builder.RegisterMultiple<IPartialTemplate, FormsLoginTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, FormsProfileTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, RedirectLinkTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, RedirectLinkProfileTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, FormsInvitationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, ForgotPasswordNotificationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, ConfirmAccountNotificationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, NewInvitationNotificationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, MailMfaLoginTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, MailMfaRegistrationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, MfaProviderListTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, MfaMailNotificationTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, ProgressSpinnerTemplate>(ServiceLifetime.Singleton);

            builder.RegisterMultiple<ITemplateHelper, RegistrationTemplateHelper>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<ITemplateHelper, TranslateTemplateHelper>(ServiceLifetime.Singleton);

            return builder;
        }

        public static IIdentityServiceBuilder AddAssets(this IIdentityServiceBuilder builder, Assembly assembly)
        {
            builder.ServiceCollection.AddSingleton<IAssetProvider, AssemblyAssetProvider>(sp => new AssemblyAssetProvider(assembly));
            return builder;
        }

        public static IIdentityServiceBuilder WithNotifications(this IIdentityServiceBuilder builder, Action<SmtpOptions> configuration = null)
        {
            builder.ReplaceService<INotificationQueue, NotificationMemoryQueue>(ServiceLifetime.Singleton);
            builder.ReplaceService<INotificationProcessor, NotificationProcessor>(ServiceLifetime.Transient);
            builder.ServiceCollection.AddHostedService<NotificationJob>();
            return builder;
        }

        public static IIdentityServiceBuilder AddSmtpMailConnector(this IIdentityServiceBuilder builder, Action<SmtpOptions> configuration = null)
        {
            if (configuration != null)
            {
                builder.ServiceCollection.Configure<SmtpOptions>(configuration);
            }

            builder.WithNotifications();
            builder.ReplaceService<IMailConnector, SmtpMailConnector>(ServiceLifetime.Scoped);

            return builder;
        }

        public static IIdentityServiceBuilder WithLoginAsEmail(this IIdentityServiceBuilder builder)
        {
            builder.ReplaceService<IMailAddressProvider, LoginNameMailAddressProvider>(ServiceLifetime.Singleton);
            builder.ReplaceService<ILoginPolicyProvider, EmailLoginPolicyProvider>(ServiceLifetime.Scoped);

            return builder;
        }
    }
}