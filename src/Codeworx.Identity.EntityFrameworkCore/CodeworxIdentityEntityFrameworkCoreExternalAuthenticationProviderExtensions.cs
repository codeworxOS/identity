using System;
using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.ExternalLogin;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreExternalAuthenticationProviderExtensions
    {
        public static IExternalLoginRegistration ToExternalLoginRegistration(this ExternalAuthenticationProvider provider, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider)
        {
            var processorType = processorTypeLookups.FirstOrDefault(t => t.Key == provider.EndpointType)?.Type;
            var processor = serviceProvider.GetService(processorType) as IExternalLoginProcessor;

            object processorConfiguration = null;

            if (provider.EndpointConfiguration != null && processor?.ConfigurationType != null)
            {
                processorConfiguration = JsonConvert.DeserializeObject(provider.EndpointConfiguration, processor.ConfigurationType);
            }

            return new ExternalLoginRegistration
            {
                Id = provider.Id.ToString("N"),
                Name = provider.Name,
                ProcessorType = processorType,
                ProcessorConfiguration = processorConfiguration
            };
        }
    }
}
