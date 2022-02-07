using System;
using System.Security.Cryptography;
using System.Text;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2HashingProvider : IHashingProvider
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        private readonly Argon2Options _options;

        public Argon2HashingProvider(Argon2Options options)
        {
            _options = options;
        }

        public string Create(string plaintext)
        {
            byte[] salt = CreateSalt(16);

            var argon2 = new Argon2(_options.Argon2Mode, _options.MemorySize, _options.Iterations, _options.HashLength);

            return argon2.CreateHash(plaintext, salt);
        }

        public bool Validate(string plaintext, string hashValue)
        {
            var argon2 = new Argon2(_options.Argon2Mode, _options.MemorySize, _options.Iterations, _options.HashLength);

            return argon2.Validate(plaintext, hashValue);
        }

        private static byte[] CreateSalt(int size)
        {
            byte[] data = new byte[4 * size];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % _chars.Length;

                result.Append(_chars[idx]);
            }

            return Encoding.UTF8.GetBytes(result.ToString());
        }
    }
}
