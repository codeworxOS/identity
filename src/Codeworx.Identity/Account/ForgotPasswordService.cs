using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Account
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IBaseUriAccessor _accessor;
        private readonly IdentityOptions _options;
        private readonly IInvitationCache _invitationCache;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public ForgotPasswordService(
            IBaseUriAccessor accessor,
            IOptionsSnapshot<IdentityOptions> options,
            IUserService userService,
            INotificationService notificationService,
            IInvitationCache invitationCache = null)
        {
            _accessor = accessor;
            _options = options.Value;
            _invitationCache = invitationCache;
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task<bool> IsSupportedAsync()
        {
            var notificationSupported = await _notificationService.IsSupportedAsync().ConfigureAwait(false);

            return _invitationCache != null && notificationSupported;
        }

        public async Task<ForgotPasswordResponse> ProcessForgotPasswordAsync(ProcessForgotPasswordRequest request)
        {
            if (!await IsSupportedAsync().ConfigureAwait(false))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Forgot password handling not supported!"));
            }

            var builder = new UriBuilder(_accessor.BaseUri);
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login");

            if (request.ReturnUrl != null)
            {
                builder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            if (request.Prompt != null)
            {
                builder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            var loginUrl = builder.ToString();

            var user = await _userService.GetUserByNameAsync(request.Login).ConfigureAwait(false);

            if (user != null)
            {
                var invitationCode = Guid.NewGuid().ToString("N");
                await _invitationCache.AddAsync(
                                        invitationCode,
                                        new Invitation.InvitationItem
                                        {
                                            RedirectUri = request.ReturnUrl,
                                            UserId = user.Identity,
                                        },
                                        TimeSpan.FromHours(1))
                    .ConfigureAwait(false);

                var invitationBuilder = new UriBuilder(_accessor.BaseUri);
                invitationBuilder.AppendPath(_options.AccountEndpoint);
                invitationBuilder.AppendPath("invitation");
                invitationBuilder.AppendPath(invitationCode);

                var invitationUrl = invitationBuilder.ToString();

                var notification = new ForgotPasswordNotification(invitationUrl, user, _options.CompanyName, _options.SupportEmail);

                await _notificationService.SendNotificationAsync(notification).ConfigureAwait(false);
            }

            return new ForgotPasswordResponse(loginUrl);
        }

        public async Task<ForgotPasswordViewResponse> ShowForgotPasswordViewAsync(ForgotPasswordRequest request)
        {
            if (!await IsSupportedAsync().ConfigureAwait(false))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Forgot password handling not supported!"));
            }

            return new ForgotPasswordViewResponse();
        }
    }
}
