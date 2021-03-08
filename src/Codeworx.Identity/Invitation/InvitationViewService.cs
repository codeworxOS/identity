using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Invitation
{
    public class InvitationViewService : IInvitationViewService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInvitationService _service;
        private readonly IEnumerable<ILoginRegistrationProvider> _providers;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public InvitationViewService(
            IServiceProvider serviceProvider,
            IInvitationService service,
            IUserService userService,
            ILoginService loginService,
            IEnumerable<ILoginRegistrationProvider> providers)
        {
            _serviceProvider = serviceProvider;
            _service = service;
            _providers = providers;
            _userService = userService;
            _loginService = loginService;
        }

        public async Task<InvitationViewResponse> ProcessAsync(InvitationViewRequest request)
        {
            InvitationViewResponse response = null;
            try
            {
                var invitation = await _service.GetInvitationAsync(request.Code);

                var user = await _userService.GetUserByIdAsync(invitation.UserId);
                var providerRequest = new ProviderRequest(invitation.RedirectUri, null, user.Name);
                var registrationInfoResponse = await _loginService.GetRegistrationInfosAsync(providerRequest);

                response = new InvitationViewResponse(registrationInfoResponse.Groups);
            }
            catch (InvitationNotFoundException)
            {
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is invalid!");
            }
            catch (InvitationExpiredException)
            {
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is expired!");
            }
            catch (InvitationCodeAlreadyUsedException)
            {
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is no longer valid!");
            }

            return response;
        }
    }
}
