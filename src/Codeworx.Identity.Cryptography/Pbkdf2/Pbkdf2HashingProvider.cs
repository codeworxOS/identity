using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Codeworx.Identity.Cryptography.Pbkdf2
{
    public class Pbkdf2HashingProvider : IHashingProvider
    {
        private readonly Pbkdf2Options _options;

        public Pbkdf2HashingProvider(Pbkdf2Options options)
        {
            _options = options;
        }

        public string Create(string plaintext)
        {
            var iterations = _options.Iterations;

            byte[] salt = new byte[_options.SaltLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hashValue = KeyDerivation.Pbkdf2(plaintext, salt, _options.HashAlgorithm, iterations, _options.OutputLength);

            return $"$pbkdf2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hashValue)}";
        }

        public bool Validate(string plaintext, string hashValue)
        {
            int iterations = 0;
            byte[] salt;
            byte[] hash;

            var input = hashValue.Split('$');
            if (input.Length != 5 || !int.TryParse(input[2], out iterations))
            {
                throw new ArgumentException("Invalid hash value format.", nameof(hashValue));
            }

            try
            {
                salt = Convert.FromBase64String(input[3]);
                hash = Convert.FromBase64String(input[4]);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hash value format.", nameof(hashValue));
            }

            var compareHash = KeyDerivation.Pbkdf2(plaintext, salt, _options.HashAlgorithm, iterations, hash.Length);

            return compareHash.SequenceEqual(hash);
        }
    }
}