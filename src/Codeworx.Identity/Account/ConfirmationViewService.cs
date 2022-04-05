using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;

namespace Codeworx.Identity.Account
{
    public class ConfirmationViewService : IConfirmationViewService
    {
        private readonly IUserService _userService;
        private readonly IStringResources _stringResources;
        private readonly IConfirmationService _confirmationService;

        public ConfirmationViewService(IUserService userService, IStringResources stringResources, IConfirmationService confirmationService)
        {
            _userService = userService;
            _stringResources = stringResources;
            _confirmationService = confirmationService;
        }

        public async Task<ConfirmationResponse> ProcessAsync(ConfirmationRequest request, CancellationToken token = default)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity).ConfigureAwait(false);

            string error = null;
            string message = null;
            try
            {
                await _confirmationService.ConfirmAsync(user, request.ConfirmationCode, token).ConfigureAwait(false);
                message = string.Format(_stringResources.GetResource(StringResource.ConfirmationMessage), user.Name);
            }
            catch (InvalidConfirmationCodeException)
            {
                error = _stringResources.GetResource(StringResource.ConfirmationCodeInvalid);
            }

            var response = new ConfirmationResponse(user, message, error);

            return response;
        }
    }
}
