using System;
using Codeworx.Identity.Configuration.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public static class CodeworxIdentityConfigurationIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseConfiguration(this IIdentityServiceBuilder builder, IConfiguration configuration)
        {
            return builder.UseConfigurationClients(configuration.GetSection(Infrastructure.Constants.IdentityConfigSectionName));
        }

        public static IIdentityServiceBuilder UseConfigurationClients(this IIdentityServiceBuilder builder, IConfigurationSection configurationSection)
        {
            builder.ServiceCollection.Configure<ClientConfigOptions>(configurationSection.GetSection(Infrastructure.Constants.ClientConfigSectionName));
            return builder.ReplaceService<IClientService, ConfigurationClientService>(ServiceLifetime.Scoped);
        }

        public static IIdentityServiceBuilder UseConfigurationClients(this IIdentityServiceBuilder builder, Action<ClientConfigOptions> clientOptions)
        {
            builder.ServiceCollection.AddOptions<ClientConfigOptions>().Configure(clientOptions);
            return builder.ReplaceService<IClientService, ConfigurationClientService>(ServiceLifetime.Scoped);
        }
    }
}