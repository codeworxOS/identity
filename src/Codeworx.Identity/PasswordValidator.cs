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
            var isValid = _hashingProvider.Validate(password, user.PasswordHash);

            return Task.FromResult(isValid);
        }
    }
}