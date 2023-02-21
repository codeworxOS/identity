using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityChangePasswordService<TContext> : IChangePasswordService
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IHashingProvider _hashing;
        private readonly IStringResources _stringResources;
        private readonly IdentityOptions _options;

        public EntityChangePasswordService(TContext context, IHashingProvider hashing, IOptions<IdentityOptions> options, IStringResources stringResources)
        {
            _context = context;
            _hashing = hashing;
            _stringResources = stringResources;
            _options = options.Value;
        }

        public async Task SetPasswordAsync(IUser user, string password)
        {
            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var hash = _hashing.Create(password);

                var userId = Guid.Parse(user.Identity);
                var userEntity = await _context.Set<User>().FirstAsync(p => p.Id == userId);

                var passwordHistoryLength = _options.PasswordHistoryLength;
                if (passwordHistoryLength > 0)
                {
                    var passwordHistorySet = _context.Set<UserPasswordHistory>();
                    var userPasswordHistory = passwordHistorySet.Where(x => x.UserId == userId)
                                                                   .OrderByDescending(x => x.ChangedAt).ToList();

                    foreach (var entry in userPasswordHistory.Take(passwordHistoryLength))
                    {
                        if (_hashing.Validate(password, entry.PasswordHash))
                        {
                            throw new PasswordChangeException(_stringResources.GetResource(StringResource.PasswordChangePasswordReuseError));
                        }
                    }

                    var passwordsToDelete = userPasswordHistory.Skip(passwordHistoryLength - 1);
                    passwordHistorySet.RemoveRange(passwordsToDelete);

                    if (userEntity.PasswordHash != null)
                    {
                        var passwordHistory = new UserPasswordHistory
                        {
                            ChangedAt = DateTime.UtcNow,
                            PasswordHash = userEntity.PasswordHash,
                            UserId = userEntity.Id,
                        };

                        passwordHistorySet.Add(passwordHistory);
                    }
                }

                userEntity.PasswordHash = hash;
                userEntity.PasswordChanged = DateTime.UtcNow;
                userEntity.FailedLoginCount = 0;
                userEntity.ForceChangePassword = false;

                var refreshToken = await _context.Set<IdentityCache>()
                                                .Where(p => p.UserId == userEntity.Id)
                                                .Where(p => !p.Disabled && p.ValidUntil >= DateTime.UtcNow)
                                                .Select(p => new IdentityCache { Value = p.Value, Disabled = p.Disabled })
                                                .ToListAsync();

                foreach (var item in refreshToken)
                {
                    _context.Entry(item).State = EntityState.Unchanged;
                    item.Disabled = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }
}