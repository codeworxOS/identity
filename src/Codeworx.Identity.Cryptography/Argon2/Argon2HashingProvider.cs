using System;
using System.Text;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2HashingProvider : IHashingProvider
    {
        private readonly Argon2Options _options;

        public Argon2HashingProvider(Argon2Options options)
        {
            _options = options;
        }

        public string Create(string plaintext)
        {
            var salt = Sodium.PasswordHash.ArgonGenerateSalt();

            var hash = Sodium.PasswordHash.ArgonHashBinary(
                Encoding.UTF8.GetBytes(plaintext),
                salt,
                _options.Iterations,
                _options.MemorySize * 1024,
                _options.HashLength,
                GetArgon2Config(_options.Argon2Mode));

            var test = Sodium.PasswordHash.ArgonHashString(plaintext, _options.Iterations, _options.MemorySize * 1024);

            var encodedSalt = Sodium.Utilities.BinaryToBase64(salt).TrimEnd('=');
            var encodedHash = Sodium.Utilities.BinaryToBase64(hash).TrimEnd('=');
            var result = $"${_options.Argon2Mode.ToString().ToLower()}$v=19$m={_options.MemorySize},t={_options.Iterations},p=1${encodedSalt}${encodedHash}";
            return result;
        }

        public bool Validate(string plaintext, string hashValue)
        {
            return Sodium.PasswordHash.ArgonHashStringVerify(hashValue, plaintext);
        }

        private Sodium.PasswordHash.ArgonAlgorithm GetArgon2Config(Argon2Mode mode)
        {
            switch (mode)
            {
                case Argon2Mode.Argon2i:
                    return Sodium.PasswordHash.ArgonAlgorithm.Argon_2I13;
                case Argon2Mode.Argon2id:
                    return Sodium.PasswordHash.ArgonAlgorithm.Argon_2ID13;
            }

            throw new NotSupportedException($"Mode {mode} not supported!");
        }
    }
}
