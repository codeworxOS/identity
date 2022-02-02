using System;
using System.Text;
using static Codeworx.Identity.Cryptography.Interop.Libsodium;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2
    {
        static Argon2()
        {
            var result = Interop.Libsodium.sodium_init();

            if (result < 0)
            {
                throw new ApplicationException($"libsodion init error: {result}");
            }
        }

        public Argon2(Argon2Mode hashMode, int memorySize, int iterations, int hashSize)
        {
            HashMode = hashMode;
            MemorySize = memorySize;
            Iterations = iterations;
            HashSize = hashSize;
        }

        public Argon2Mode HashMode { get; }

        public int HashSize { get; }

        public int Iterations { get; }

        public int MemorySize { get; }

        public string CreateHash(string password, byte[] salt)
        {
            var buffer = Encoding.UTF8.GetBytes(password);

            var hash = new byte[HashSize];
            var result = crypto_pwhash(hash, (ulong)HashSize, buffer, (ulong)buffer.Length, salt, (ulong)Iterations, (uint)MemorySize * 1024, GetAlg());

            if (result != 0)
            {
                throw new HashingException($"Error creating argon2 hash: {result}");
            }

            var encodedSalt = Convert.ToBase64String(salt).TrimEnd('=');
            var encodedHash = Convert.ToBase64String(hash).TrimEnd('=');

            var hashPhrase = $"${HashMode.ToString().ToLower()}$v=19$m={MemorySize},t={Iterations},p=1${encodedSalt}${encodedHash}";
            return hashPhrase;
        }

        public bool Validate(string password, string argonHash)
        {
            var hash = Encoding.UTF8.GetBytes(argonHash);
            var buffer = Encoding.UTF8.GetBytes(password);

            var result = Interop.Libsodium.crypto_pwhash_str_verify(hash, buffer, (ulong)buffer.Length);

            return result == 0;
        }

        private int GetAlg()
        {
            return this.HashMode switch
            {
                Argon2Mode.Argon2i => crypto_pwhash_argon2i_ALG_ARGON2I13,
                Argon2Mode.Argon2id => crypto_pwhash_argon2id_ALG_ARGON2ID13,
                _ => throw new NotSupportedException($"Argon2 mode {this.HashMode} is currently not supported!"),
            };
        }
    }
}
