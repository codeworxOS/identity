using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public class ProfileService : IProfileService
    {
        private readonly ImmutableList<ILoginRegistrationProvider> _providers;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public ProfileService(IUserService userService, IEnumerable<ILoginRegistrationProvider> providers, IServiceProvider serviceProvider)
        {
            _providers = providers.ToImmutableList();
            _userService = userService;
            _serviceProvider = serviceProvider;
        }

        public async Task<ProfileResponse> ProcessAsync(ProfileRequest request)
        {
            var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());

            var providerRequest = new ProviderRequest(ProviderRequestType.Profile, request.ReturnUrl, user: user);

            if (request.LoginProviderId != null)
            {
                providerRequest.ProviderErrors.Add(request.LoginProviderId, request.LoginProviderError);
            }

            var infos = new List<ILoginRegistrationInfo>();
            var groups = new List<ILoginRegistrationGroup>();

            foreach (var provider in _providers)
            {
                foreach (var registration in await provider.GetLoginRegistrationsAsync())
                {
                    var processor = (ILoginProcessor)_serviceProvider.GetService(registration.ProcessorType);
                    var info = await processor.GetRegistrationInfoAsync(providerRequest, registration);
                    infos.Add(info);
                }
            }

            foreach (var item in infos.GroupBy(p => p.Template).OrderBy(p => p.Key))
            {
                groups.Add(new LoginRegistrationGroup(item.Key, item.Select(x => x)));
            }

            return new ProfileResponse(user, groups, request.ReturnUrl);
        }
    }
}
