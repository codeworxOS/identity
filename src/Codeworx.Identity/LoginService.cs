using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity
{
    // EventIds 143xx
    public class LoginService : ILoginService
    {
        private static readonly Action<ILogger, string, Exception> _processorNotFoundMessage;
        private static readonly Action<ILogger, string, Exception> _loginFailed;
        private static readonly Action<ILogger, ProviderRequestType, string, string, string, Exception> _getRegistrationInfos;
        private static readonly Action<ILogger, string, Exception> _getLoginRegistrationInfo;
        private static readonly Action<ILogger, string, Exception> _getParameterType;
        private static readonly Action<ILogger, string, Exception> _signIn;
        private static readonly Action<ILogger, string, Exception> _unhandledErrorProcessingLogin;
        private readonly IEnumerable<ILoginRegistrationProvider> _providers;
        private readonly IIdentityService _service;
        private readonly ILogger<LoginService> _logger;
        private readonly IStringResources _resources;
        private readonly IServiceProvider _serviceProvider;

        static LoginService()
        {
            _processorNotFoundMessage = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14301), "Provider {providerId} not found!");
            _loginFailed = LoggerMessage.Define<string>(LogLevel.Information, new EventId(14302), "Login failed for provider {provider}");
            _getRegistrationInfos = LoggerMessage.Define<ProviderRequestType, string, string, string>(
                LogLevel.Information,
                new EventId(14303),
                "GetRegistrationInfosAsync: {requestType}, {returnUrl}, {prompt}, {username}");
            _getLoginRegistrationInfo = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(14304),
                "GetLoginRegistrationInfoAsync: {providerId}");
            _getParameterType = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(14305),
                "GetParameterTypeAsync: {providerId}");
            _signIn = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(14306),
                "SignInAsync: {providerId}");
            _unhandledErrorProcessingLogin = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(14307),
                "There was an unhandled error Processing the login for provider {providerId}.");
        }

        public LoginService(IEnumerable<ILoginRegistrationProvider> providers, IServiceProvider serviceProvider, IIdentityService service, ILogger<LoginService> logger, IStringResources resources)
        {
            _providers = providers;
            _serviceProvider = serviceProvider;
            _service = service;
            _logger = logger;
            _resources = resources;
        }

        public async Task<ILoginRegistration> GetLoginRegistrationInfoAsync(string providerId)
        {
            _getLoginRegistrationInfo(_logger, providerId, null);

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
            _getParameterType(_logger, providerId, null);

            var processorInfo = await GetProcessorInfoAsync(providerId);

            return processorInfo.Processor.RequestParameterType;
        }

        public async Task<RegistrationInfoResponse> GetRegistrationInfosAsync(ProviderRequest request)
        {
            _getRegistrationInfos(_logger, request.Type, request.ReturnUrl, request.Prompt, request.UserName, null);

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

                    var infos = groups.GetOrAdd(info.Template, p => new List<ILoginRegistrationInfo>());
                    infos.Add(info);
                }
            }

            return new RegistrationInfoResponse(groups.OrderBy(p => p.Key).Select(p => new LoginRegistrationGroup(p.Key, p.Value)));
        }

        public async Task<SignInResponse> SignInAsync(string providerId, object parameter)
        {
            _signIn(_logger, providerId, null);

            try
            {
                var processorInfo = await GetProcessorInfoAsync(providerId);
                var response = await processorInfo.Processor.ProcessAsync(processorInfo.Registration, parameter).ConfigureAwait(false);
                return response;
            }
            catch (ReturnUrlException exception)
            {
                LogSignInException(exception.InnerException, providerId);
                throw;
            }
            catch (Exception ex)
            {
                LogSignInException(ex, providerId);
                throw;
            }
        }

        private void LogSignInException(Exception ex, string providerId)
        {
            if (ex is AuthenticationException authException)
            {
                _loginFailed(_logger, providerId, authException);
            }
            else if (ex is LoginProviderNotFoundException)
            {
                _processorNotFoundMessage(_logger, providerId, null);
            }
            else
            {
                _unhandledErrorProcessingLogin(_logger, providerId, ex);
            }
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

            throw new LoginProviderNotFoundException(providerId, _resources.GetResource(StringResource.UnknownLoginProviderError));
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