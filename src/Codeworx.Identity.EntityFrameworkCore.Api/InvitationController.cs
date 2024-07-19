using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/users/{id:guid}/invitations")]
    [Authorize(Policy = Policies.Admin)]
    public class InvitationController
    {
        private readonly IContextWrapper _db;
        private readonly IdentityServerOptions _serverOptions;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IdentityOptions _options;

        public InvitationController(IContextWrapper db, IOptionsSnapshot<IdentityOptions> options, IdentityServerOptions serverOptions, IBaseUriAccessor baseUriAccessor, IUserService userService, INotificationService notificationService)
        {
            _db = db;
            _serverOptions = serverOptions;
            _baseUriAccessor = baseUriAccessor;
            _userService = userService;
            _notificationService = notificationService;
            _options = options.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status501NotImplemented)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<string> CreateInvitationAsync(Guid id, [FromBody] InvitationInsertData invitation)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var userExists = await _db.Context.Set<User>().AnyAsync(p => p.Id == id).ConfigureAwait(false);

                if (!userExists)
                {
                    // TODO return 404
                    throw new NotSupportedException();
                }

                var code = invitation.Code ?? Guid.NewGuid().ToString("N");

                if (invitation.Code != null)
                {
                    var codeExists = await _db.Context.Set<UserInvitation>().AnyAsync(p => p.InvitationCode == code).ConfigureAwait(false);

                    if (codeExists)
                    {
                        // TODO return 409
                        throw new NotSupportedException();
                    }
                }

                var activeInvitations = await _db.Context.Set<UserInvitation>()
                    .Where(p => p.UserId == id && !p.IsDisabled && p.ValidUntil > DateTime.UtcNow)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var item in activeInvitations)
                {
                    item.IsDisabled = true;
                }

                var action = invitation.Action;

                if (action == InvitationAction.None)
                {
                    action = InvitationAction.Initial;
                }

                var entity = new UserInvitation
                {
                    UserId = id,
                    InvitationCode = code,
                    RedirectUri = invitation.RedriectUri,
                    Action = action,
                    ValidUntil = DateTime.UtcNow.Add(invitation.ValidFor ?? _options.InvitationValidity),
                    IsDisabled = false,
                };

                _db.Context.Add(entity);

                await _db.Context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                if (invitation.SendNotification ?? false)
                {
                    var isSupported = await _notificationService.IsSupportedAsync();

                    if (!isSupported)
                    {
                        // TODO return 409
                        throw new NotSupportedException();
                    }

                    var user = await _userService.GetUserByIdAsync(id.ToString("N")).ConfigureAwait(false);

                    var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
                    uriBuilder.AppendPath(_serverOptions.AccountEndpoint);
                    uriBuilder.AppendPath("invitation");
                    uriBuilder.AppendPath(code);

                    var notification = new NewInvitationNotification(uriBuilder.ToString(), code, user, _options.CompanyName, _options.SupportEmail);

                    await _notificationService.SendNotificationAsync(notification).ConfigureAwait(false);
                }

                return code;
            }
        }

        [HttpPut("disable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task DisableInvitationAsync(Guid id)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var invitations = await _db.Context
                                            .Set<UserInvitation>()
                                            .Where(p => !p.IsDisabled)
                                            .ToListAsync().ConfigureAwait(false);

                if (!invitations.Any())
                {
                    // TODO return 404
                }

                foreach (var item in invitations)
                {
                    item.IsDisabled = true;
                }

                await _db.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }
}
