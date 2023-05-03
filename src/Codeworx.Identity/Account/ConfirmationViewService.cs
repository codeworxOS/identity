using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;

namespace Codeworx.Identity.Account
{
    public class ConfirmationViewService : IConfirmationViewService
    {
        private readonly IUserService _userService;
        private readonly IStringResources _stringResources;
        private readonly IConfirmationService _confirmationService;
        private readonly IIdentityService _identityService;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IdentityServerOptions _options;

        public ConfirmationViewService(
            IUserService userService,
            IStringResources stringResources,
            IConfirmationService confirmationService,
            IIdentityService identityService,
            IBaseUriAccessor baseUriAccessor,
            IdentityServerOptions options)
        {
            _userService = userService;
            _stringResources = stringResources;
            _confirmationService = confirmationService;
            _identityService = identityService;
            _baseUriAccessor = baseUriAccessor;
            _options = options;
        }

        public async Task<ConfirmationResponse> ProcessAsync(ConfirmationRequest request, CancellationToken token = default)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity).ConfigureAwait(false);

            string error = null;
            string message = null;
            ClaimsIdentity identity = null;
            try
            {
                await _confirmationService.ConfirmAsync(user, request.ConfirmationCode, token).ConfigureAwait(false);
                message = string.Format(_stringResources.GetResource(StringResource.ConfirmationMessage), user.Name);
                user = await _userService.GetUserByIdAsync(user.Identity).ConfigureAwait(false);
                identity = await _identityService.GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);
            }
            catch (InvalidConfirmationCodeException)
            {
                error = _stringResources.GetResource(StringResource.ConfirmationCodeInvalid);
            }

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login");
            uriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, Constants.OAuth.Prompt.SelectAccount);

            var response = new ConfirmationResponse(user, identity, uriBuilder.ToString(), message, error, request.RememberMe);

            return response;
        }
    }
}
