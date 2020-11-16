using Codeworx.Identity.Configuration.Infrastructure;
using Codeworx.Identity.Login;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public static class CodeworxIdentityConfigurationIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseConfiguration(this IIdentityServiceBuilder builder, IConfiguration configuration)
        {
            return builder.UseConfiguration(configuration.GetSection(Infrastructure.Constants.IdentityConfigSectionName));
        }

        public static IIdentityServiceBuilder UseConfiguration(this IIdentityServiceBuilder builder, IConfigurationSection section)
        {
            return builder
                        .UseConfigurationClients(section.GetSection(Infrastructure.Constants.ClientConfigSectionName))
                        .UseConfigurationLoginRegistrations(section.GetSection(Infrastructure.Constants.LoginRegistrationConfigSectionName))
                        .UseConfigurationUsers(section.GetSection(Infrastructure.Constants.UserConfigSectionName));
        }

        public static IIdentityServiceBuilder UseConfigurationClients(this IIdentityServiceBuilder builder, IConfigurationSection configurationSection)
        {
            builder.ServiceCollection.Configure<ClientConfigOptions>(configurationSection);

            return builder.Clients<ConfigurationClientService>(ServiceLifetime.Scoped);
        }

        public static IIdentityServiceBuilder UseConfigurationUsers(this IIdentityServiceBuilder builder, IConfigurationSection configurationSection)
        {
            builder.ServiceCollection.Configure<UserConfigOptions>(configurationSection);

            return builder.Users<ConfigurationUserService>(ServiceLifetime.Singleton);
        }

        public static IIdentityServiceBuilder UseConfigurationLoginRegistrations(this IIdentityServiceBuilder builder, IConfigurationSection configurationSection)
        {
            return builder.LoginRegistrations<ConfigurationLoginRegistrationProvider>(
                ServiceLifetime.Singleton,
                sp => new ConfigurationLoginRegistrationProvider(configurationSection, sp.GetServices<IProcessorTypeLookup>(), sp));
        }
    }
}