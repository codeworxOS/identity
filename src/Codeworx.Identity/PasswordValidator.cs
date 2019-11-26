using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class PasswordValidator : IPasswordValidator
    {
        private readonly IHashingProvider _hashingProvider;

        public PasswordValidator(IHashingProvider hashingProvider)
        {
            _hashingProvider = hashingProvider;
        }

        public Task<bool> Validate(IUser user, string password)
        {
            var hashed = _hashingProvider.Hash(password, user.PasswordSalt);
            return Task.FromResult(hashed.SequenceEqual(user.PasswordHash));
        }
    }
}