using System;
using System.Reflection;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Mail;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddAssets(this IIdentityServiceBuilder builder, Assembly assembly)
        {
            builder.ServiceCollection.AddSingleton<IAssetProvider, AssemblyAssetProvider>(sp => new AssemblyAssetProvider(assembly));
            return builder;
        }

        public static IIdentityServiceBuilder AddSmtpMailConnector(this IIdentityServiceBuilder builder, Action<SmtpOptions> configuration = null)
        {
            if (configuration != null)
            {
                builder.ServiceCollection.Configure<SmtpOptions>(configuration);
            }

            builder.ReplaceService<IMailConnector, SmtpMailConnector>(ServiceLifetime.Singleton);

            return builder;
        }
    }
}