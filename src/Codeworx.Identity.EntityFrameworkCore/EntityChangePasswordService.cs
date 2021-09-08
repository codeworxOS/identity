using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityChangePasswordService<TContext> : IChangePasswordService
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IHashingProvider _hashing;

        public EntityChangePasswordService(TContext context, IHashingProvider hashing)
        {
            _context = context;
            _hashing = hashing;
        }

        public async Task SetPasswordAsync(IUser user, string password)
        {
            var hash = _hashing.Create(password);

            var userId = Guid.Parse(user.Identity);
            var userEntity = await _context.Set<User>().FirstAsync(p => p.Id == userId);
            userEntity.PasswordHash = hash;
            userEntity.PasswordChanged = DateTime.UtcNow;
            userEntity.FailedLoginCount = 0;
            userEntity.ForceChangePassword = false;

            var refreshToken = await _context.Set<UserRefreshToken>()
                                            .Where(p => p.UserId == userEntity.Id)
                                            .Where(p => !p.IsDisabled && p.ValidUntil >= DateTime.UtcNow)
                                            .ToListAsync();

            foreach (var item in refreshToken)
            {
                item.IsDisabled = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}