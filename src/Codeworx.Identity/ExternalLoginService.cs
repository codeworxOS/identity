using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IEnumerable<IExternalLoginProvider> _providers;
        private readonly IEnumerable<IProcessorTypeLookup> _processorTypeLookups;
        private readonly IIdentityService _service;
        private readonly IServiceProvider _serviceProvider;

        public ExternalLoginService(IEnumerable<IExternalLoginProvider> providers, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider, IIdentityService service)
        {
            _providers = providers;
            _processorTypeLookups = processorTypeLookups;
            _serviceProvider = serviceProvider;
            _service = service;
        }

        public async Task<Type> GetParameterTypeAsync(string providerId)
        {
            var processor = await GetProcessorAsync(providerId);

            return processor.RequestParameterType;
        }

        public async Task<IExternalLoginProcessor> GetProcessorAsync(string providerId)
        {
            foreach (var item in _providers)
            {
                var registrations = await item.GetLoginRegistrationsAsync();

                foreach (var registration in registrations)
                {
                    if (registration.Id == providerId)
                    {
                        var processorType = _processorTypeLookups.FirstOrDefault(p => p.Key == registration.ProcessorKey)?.Type;
                        var processor = _serviceProvider.GetRequiredService(processorType) as IExternalLoginProcessor;

                        return processor;
                    }
                }
            }

            throw new KeyNotFoundException($"Provider {providerId} not found!");
        }

        public async Task<ProviderInfosResponse> GetProviderInfosAsync(ProviderRequest request)
        {
            var result = new List<ExternalProviderInfo>();

            foreach (var item in _providers)
            {
                foreach (var externalLogin in await item.GetLoginRegistrationsAsync(request.UserName))
                {
                    var processorType = _processorTypeLookups.FirstOrDefault(p => p.Key == externalLogin.ProcessorKey)?.Type;
                    var processor = _serviceProvider.GetService(processorType) as IExternalLoginProcessor;

                    var info = new ExternalProviderInfo
                    {
                        Id = externalLogin.Id,
                        Name = externalLogin.Name,
                        Url = await processor.GetProcessorUrlAsync(request, externalLogin.ProcessorConfiguration)
                    };
                    result.Add(info);
                }
            }

            return new ProviderInfosResponse(result);
        }

        public async Task<SignInResponse> SignInAsync(string providerId, object parameter)
        {
            var processor = await GetProcessorAsync(providerId);

            var response = await processor.ProcessAsync(parameter);
            var identityData = await _service.LoginExternalAsync(providerId, response.NameIdentifier);

            return new SignInResponse(identityData, response.ReturnUrl);
        }
    }
}