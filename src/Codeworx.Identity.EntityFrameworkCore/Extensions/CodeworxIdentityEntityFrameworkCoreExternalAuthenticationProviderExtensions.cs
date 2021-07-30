using System;
using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreExternalAuthenticationProviderExtensions
    {
        public static ILoginRegistration ToExternalLoginRegistration(this AuthenticationProvider provider, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider)
        {
            var processorType = processorTypeLookups.FirstOrDefault(t => t.Key == provider.EndpointType);
            var processor = serviceProvider.GetRequiredService(processorType.Type) as ILoginProcessor;

            object processorConfiguration = null;

            if (provider.EndpointConfiguration != null && processorType?.ConfigurationType != null)
            {
                processorConfiguration = JsonConvert.DeserializeObject(provider.EndpointConfiguration, processorType.ConfigurationType);
            }

            return new ExternalLoginRegistration
            {
                Id = provider.Id.ToString("N").ToLower(),
                Name = provider.Name,
                ProcessorType = processorType.Type,
                ProcessorConfiguration = processorConfiguration,
            };
        }
    }
}
