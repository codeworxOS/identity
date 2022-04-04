using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore.Account
{
    public class EntityConfirmationService<TContext> : IConfirmationService
        where TContext : DbContext
    {
        private readonly TContext _db;
        private readonly IdentityOptions _options;
        private readonly INotificationService _notificationService;
        private readonly IBaseUriAccessor _accessor;

        public EntityConfirmationService(
            TContext db,
            IOptionsSnapshot<IdentityOptions> options,
            INotificationService notificationService,
            IBaseUriAccessor accessor)
        {
            _db = db;
            _options = options.Value;
            _notificationService = notificationService;
            _accessor = accessor;
        }

        public async Task ConfirmAsync(IUser user, string code, CancellationToken token = default)
        {
            var userId = Guid.Parse(user.Identity);
            var userData = await _db.Set<User>().Where(p => p.Id == userId).FirstOrDefaultAsync(token).ConfigureAwait(false);
            if (userData != null)
            {
                if (userData.ConfirmationCode != code)
                {
                    throw new InvalidConfirmationCodeException();
                }

                userData.ConfirmationCode = null;
                userData.ConfirmationPending = false;
            }

            await _db.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task RequireConfirmationAsync(IUser user, CancellationToken token = default)
        {
            var userId = Guid.Parse(user.Identity);
            var userData = await _db.Set<User>().Where(p => p.Id == userId).FirstOrDefaultAsync(token).ConfigureAwait(false);
            if (userData != null)
            {
                userData.ConfirmationCode = Guid.NewGuid().ToString("N");
                userData.ConfirmationPending = true;
            }

            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            var isSupported = await _notificationService.IsSupportedAsync().ConfigureAwait(false);

            if (isSupported)
            {
                var confirmationBuilder = new UriBuilder(_accessor.BaseUri);
                confirmationBuilder.AppendPath(_options.AccountEndpoint);
                confirmationBuilder.AppendPath("confirm");
                confirmationBuilder.AppendPath(userData.ConfirmationCode);

                var invitationUrl = confirmationBuilder.ToString();

                var confirmAccount = new ConfirmAccountNotification(invitationUrl, userData.ConfirmationCode, user, _options.CompanyName, _options.SupportEmail);

                await _notificationService.SendNotificationAsync(confirmAccount).ConfigureAwait(false);
            }
        }
    }
}
