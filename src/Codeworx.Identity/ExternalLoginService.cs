using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IEnumerable<IExternalLoginProvider> _providers;
        private readonly IIdentityService _service;
        private readonly IServiceProvider _serviceProvider;

        public ExternalLoginService(IEnumerable<IExternalLoginProvider> providers, IServiceProvider serviceProvider, IIdentityService service)
        {
            _providers = providers;
            _serviceProvider = serviceProvider;
            _service = service;
        }

        public async Task<Type> GetParameterTypeAsync(string providerId)
        {
            var processorInfo = await GetProcessorInfoAsync(providerId);

            return processorInfo.Item1.RequestParameterType;
        }

        public async Task<Tuple<IExternalLoginProcessor, object>> GetProcessorInfoAsync(string providerId)
        {
            foreach (var item in _providers)
            {
                var registrations = await item.GetLoginRegistrationsAsync();

                foreach (var registration in registrations)
                {
                    if (registration.Id == providerId)
                    {
                        var processor = _serviceProvider.GetRequiredService(registration.ProcessorType) as IExternalLoginProcessor;

                        return new Tuple<IExternalLoginProcessor, object>(processor, registration.ProcessorConfiguration);
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
                    var processor = _serviceProvider.GetService(externalLogin.ProcessorType) as IExternalLoginProcessor;

                    if (processor == null)
                    {
                        return null;
                    }

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
            var processorInfo = await GetProcessorInfoAsync(providerId);

            var response = await processorInfo.Item1.ProcessAsync(parameter, processorInfo.Item2);
            var identityData = await _service.LoginExternalAsync(providerId, response.NameIdentifier);

            return new SignInResponse(identityData, response.ReturnUrl);
        }
    }
}