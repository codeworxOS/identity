using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity
{
    public class ExternalLoginService : ILoginService
    {
        private readonly IEnumerable<ILoginRegistrationProvider> _providers;
        private readonly IIdentityService _service;
        private readonly IServiceProvider _serviceProvider;

        public ExternalLoginService(IEnumerable<ILoginRegistrationProvider> providers, IServiceProvider serviceProvider, IIdentityService service)
        {
            _providers = providers;
            _serviceProvider = serviceProvider;
            _service = service;
        }

        public async Task<ILoginRegistration> GetLoginRegistrationInfosAsync(string providerId)
        {
            foreach (var item in _providers)
            {
                foreach (var externalLogin in await item.GetLoginRegistrationsAsync())
                {
                    if (externalLogin.Id == providerId)
                    {
                        return externalLogin;
                    }
                }
            }

            return null;
        }

        public async Task<Type> GetParameterTypeAsync(string providerId)
        {
            var processorInfo = await GetProcessorInfoAsync(providerId);

            return processorInfo.Processor.RequestParameterType;
        }

        public async Task<RegistrationInfoResponse> GetRegistrationInfosAsync(ProviderRequest request)
        {
            var groups = new ConcurrentDictionary<string, List<ILoginRegistrationInfo>>();

            foreach (var item in _providers)
            {
                foreach (var externalLogin in await item.GetLoginRegistrationsAsync(request.UserName))
                {
                    var processor = _serviceProvider.GetService(externalLogin.ProcessorType) as ILoginProcessor;

                    if (processor == null)
                    {
                        throw new ProcessorNotRegisteredException();
                    }

                    var info = await processor.GetRegistrationInfoAsync(request, externalLogin);

                    var infos = groups.GetOrAdd(processor.Template, p => new List<ILoginRegistrationInfo>());

                    infos.Add(info);
                }
            }

            return new RegistrationInfoResponse(groups.OrderBy(p => p.Key).Select(p => new LoginRegistrationGroup(p.Key, p.Value)));
        }

        public async Task<SignInResponse> SignInAsync(string providerId, ILoginRequest parameter)
        {
            var processorInfo = await GetProcessorInfoAsync(providerId);

            var response = await processorInfo.Processor.ProcessAsync(parameter, processorInfo.Registration).ConfigureAwait(false);

            return response;
        }

        private async Task<ProcessorInfo> GetProcessorInfoAsync(string providerId)
        {
            foreach (var item in _providers)
            {
                IEnumerable<ILoginRegistration> registrations = await item.GetLoginRegistrationsAsync();

                foreach (var registration in registrations)
                {
                    if (registration.Id == providerId)
                    {
                        var processor = _serviceProvider.GetRequiredService(registration.ProcessorType) as ILoginProcessor;

                        return new ProcessorInfo(processor, registration);
                    }
                }
            }

            throw new KeyNotFoundException($"Provider {providerId} not found!");
        }

        private class ProcessorInfo
        {
            public ProcessorInfo(ILoginProcessor processor, ILoginRegistration registration)
            {
                Processor = processor;
                Registration = registration;
            }

            public ILoginProcessor Processor
            {
                get;
            }

            public ILoginRegistration Registration
            {
                get;
            }
        }
    }
}