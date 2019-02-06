using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityPasswordValidator<TContext> : IPasswordValidator
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IHashingProvider _hashingProvider;

        public EntityPasswordValidator(TContext context, IHashingProvider hashingProvider)
        {
            _context = context;
            _hashingProvider = hashingProvider;
        }

        public async Task<bool> Validate(IUser user, string password)
        {
            var id = Guid.Parse(user.Identity);

            var currentUser = await _context.Set<User>().FirstOrDefaultAsync(p => p.Id == id);
            if (currentUser != null)
            {
                var hashed = _hashingProvider.Hash(password, currentUser.PasswordSalt);
                return hashed.SequenceEqual(currentUser.PasswordHash);
            }

            return false;
        }
    }
}