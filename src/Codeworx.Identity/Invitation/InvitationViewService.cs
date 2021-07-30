using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IIdentityService _identityService;
        private readonly IPasswordPolicyProvider _passwordPolicyProvider;
        private readonly IChangePasswordService _changePasswordService;

        public InvitationViewService(
            IServiceProvider serviceProvider,
            IInvitationService service,
            IUserService userService,
            ILoginService loginService,
            IIdentityService identityService,
            IPasswordPolicyProvider passwordPolicyProvider,
            IChangePasswordService changePasswordService,
            IEnumerable<ILoginRegistrationProvider> providers)
        {
            _serviceProvider = serviceProvider;
            _service = service;
            _providers = providers;
            _userService = userService;
            _loginService = loginService;
            _identityService = identityService;
            _passwordPolicyProvider = passwordPolicyProvider;
            _changePasswordService = changePasswordService;
        }

        public async Task<SignInResponse> ProcessAsync(ProcessInvitationViewRequest request)
        {
            var policy = await _passwordPolicyProvider.GetPolicyAsync();
            string error = null;
            bool hasError = false;

            if (request.Password != request.ConfirmPassword)
            {
                error = "Passwords do not match!";
                hasError = true;
            }
            else if (!Regex.IsMatch(request.Password, policy.Regex))
            {
                error = policy.Description;
                hasError = true;
            }

            if (hasError)
            {
                var errorResponse = await ShowAsync(new InvitationViewRequest(request.Code, request.ProviderId, error));
                throw new ErrorResponseException<InvitationViewResponse>(errorResponse);
            }

            try
            {
                var invitation = await _service.RedeemInvitationAsync(request.Code);
                var user = await _userService.GetUserByIdAsync(invitation.UserId);
                await _changePasswordService.SetPasswordAsync(user, request.Password);
                var identity = await _identityService.GetClaimsIdentityFromUserAsync(user);

                return new SignInResponse(identity, invitation.RedirectUri);
            }
            catch (InvitationNotFoundException)
            {
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is invalid!");
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (InvitationExpiredException)
            {
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is expired!");
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (InvitationAlreadyRedeemedException)
            {
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is no longer valid!");
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
        }

        public async Task<InvitationViewResponse> ShowAsync(InvitationViewRequest request)
        {
            InvitationViewResponse response = null;
            try
            {
                var invitation = await _service.GetInvitationAsync(request.Code);

                var user = await _userService.GetUserByIdAsync(invitation.UserId);
                var providerRequest = new ProviderRequest(ProviderRequestType.Invitation, invitation.RedirectUri, null, user.Name, request.Code);

                if (!string.IsNullOrWhiteSpace(request.Provider))
                {
                    providerRequest.ProviderErrors.Add(request.Provider, request.Error);
                }

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
            catch (InvitationAlreadyRedeemedException)
            {
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), "The invitation code is no longer valid!");
            }

            return response;
        }
    }
}
