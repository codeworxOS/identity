using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Codeworx.Identity.Cryptography
{
    public class Pbkdf2HasingProvider : IHashingProvider
    {
        private readonly Pbkdf2Options _options;

        public Pbkdf2HasingProvider(Pbkdf2Options options)
        {
            _options = options;
        }

        public byte[] CrateSalt()
        {
            byte[] salt = new byte[_options.SaltLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public byte[] Hash(string text, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(text, salt, _options.HashAlgorithm, _options.Iterations, _options.OutputLength);
        }
    }
}