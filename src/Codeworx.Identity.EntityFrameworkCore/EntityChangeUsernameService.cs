using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityChangeUsernameService<TContext> : IChangeUsernameService
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IStringResources _stringResources;
        private readonly IConfirmationService _confirmationService;
        private readonly IUserService _userService;
        private readonly IdentityOptions _options;

        public EntityChangeUsernameService(
            TContext context,
            IStringResources stringResources,
            IConfirmationService confirmationService,
            IUserService userService,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _context = context;
            _stringResources = stringResources;
            _confirmationService = confirmationService;
            _userService = userService;
            _options = options.Value;
        }

        public async Task<IUser> ChangeUsernameAsync(IUser user, string username, CancellationToken token = default)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var userId = Guid.Parse(user.Identity);
                var userEntity = await _context.Set<User>().FirstAsync(p => p.Id == userId);
                var exists = await _context.Set<User>().AnyAsync(p => p.Id != userId && p.Name == username).ConfigureAwait(false);

                if (exists)
                {
                    throw new UsernameAlreadyExistsException();
                }

                userEntity.Name = username;
                await _context.SaveChangesAsync().ConfigureAwait(false);

                user = await _userService.GetUserByIdAsync(user.Identity).ConfigureAwait(false);

                if (_options.EnableAccountConfirmation)
                {
                    if (_confirmationService == null)
                    {
                        throw new NotSupportedException("Missing IConfirmationService!");
                    }

                    await _confirmationService.RequireConfirmationAsync(user, token).ConfigureAwait(false);
                }

                await transaction.CommitAsync(token).ConfigureAwait(false);

                return user;
            }
        }
    }
}