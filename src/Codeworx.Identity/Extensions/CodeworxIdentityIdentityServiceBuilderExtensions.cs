using System;
using System.Reflection;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Notification;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddAssets(this IIdentityServiceBuilder builder, Assembly assembly)
        {
            builder.ServiceCollection.AddSingleton<IAssetProvider, AssemblyAssetProvider>(sp => new AssemblyAssetProvider(assembly));
            return builder;
        }

        public static IIdentityServiceBuilder WithNotifications(this IIdentityServiceBuilder builder, Action<SmtpOptions> configuration = null)
        {
            builder.ReplaceService<INotificationQueue, NotificationMemoryQueue>(ServiceLifetime.Singleton);
            builder.ReplaceService<INotificationProcessor, NotificationProcessor>(ServiceLifetime.Singleton);
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