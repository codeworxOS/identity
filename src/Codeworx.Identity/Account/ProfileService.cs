using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public class ProfileService : IProfileService
    {
        private readonly ImmutableList<ILoginRegistrationProvider> _providers;
        private readonly IUserService _userService;
        private readonly ILinkUserService _linkUserService;
        private readonly IdentityServerOptions _options;
        private readonly IInvitationCache _invitationCache;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public ProfileService(
            IUserService userService,
            ILinkUserService linkUserService,
            IEnumerable<ILoginRegistrationProvider> providers,
            IInvitationCache invitationCache,
            IServiceProvider serviceProvider,
            IdentityServerOptions options,
            IBaseUriAccessor baseUriAccessor)
        {
            _providers = providers.ToImmutableList();
            _userService = userService;
            _linkUserService = linkUserService;
            _options = options;
            _invitationCache = invitationCache;
            _serviceProvider = serviceProvider;
            _baseUriAccessor = baseUriAccessor;
        }

        public async Task<ProfileResponse> ProcessAsync(ProfileRequest request)
        {
            var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());

            var providerRequest = new ProviderRequest(ProviderRequestType.Profile, request.HeaderOnly, request.ReturnUrl, user: user);

            if (request.LoginProviderId != null)
            {
                providerRequest.ProviderErrors.Add(request.LoginProviderId, request.LoginProviderError);
            }

            var infos = new List<ILoginRegistrationInfo>();
            var groups = new List<ILoginRegistrationGroup>();

            foreach (var provider in _providers)
            {
                foreach (var registration in await provider.GetLoginRegistrationsAsync(LoginProviderType.Login))
                {
                    var processor = (ILoginProcessor)_serviceProvider.GetService(registration.ProcessorType);
                    var info = await processor.GetRegistrationInfoAsync(providerRequest, registration);
                    if (info != null)
                    {
                        infos.Add(info);
                    }
                }
            }

            foreach (var item in infos.GroupBy(p => p.Template).OrderBy(p => p.Key))
            {
                groups.Add(new LoginRegistrationGroup(item.Key, item.Select(x => x)));
            }

            return new ProfileResponse(user, groups, request.ReturnUrl);
        }

        public async Task<ProfileLinkResponse> ProcessLinkAsync(ProfileLinkRequest request)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            uriBuilder.AppendPath($"{_options.AccountEndpoint}/me");

            if (request.Direction == LinkDirection.Link)
            {
                var invitationCode = Guid.NewGuid().ToString("N");

                var invatation = new InvitationItem { RedirectUri = uriBuilder.ToString(), UserId = request.Identity.GetUserId(), Action = InvitationAction.LinkUnlink };

                await _invitationCache.AddAsync(
                                    invitationCode,
                                    invatation,
                                    TimeSpan.FromMinutes(5));

                foreach (var item in _providers)
                {
                    foreach (var configuration in await item.GetLoginRegistrationsAsync(LoginProviderType.Login))
                    {
                        if (configuration.Id == request.ProviderId)
                        {
                            var processor = (ILoginProcessor)_serviceProvider.GetService(configuration.ProcessorType);
                            var info = await processor.GetRegistrationInfoAsync(new ProviderRequest(ProviderRequestType.Invitation, request.HeaderOnly, invitationCode: invitationCode, invitation: invatation), configuration);
                            if (info.HasRedirectUri(out var redirectUri))
                            {
                                return new ProfileLinkResponse(redirectUri);
                            }
                        }
                    }
                }

                throw new NotSupportedException();
            }
            else
            {
                var user = await _userService.GetUserByIdAsync(request.Identity.GetUserId());
                await _linkUserService.UnlinkUserAsync(user, request.ProviderId);

                return new ProfileLinkResponse(uriBuilder.ToString());
            }
        }
    }
}
