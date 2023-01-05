using System;
using System.Text;
using Codeworx.Identity.Cryptography.Interop;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2
    {
        private static readonly int _sodiumInit;

        static Argon2()
        {
            _sodiumInit = Interop.NativeMethods.sodium_init();
        }

        public Argon2(Argon2Mode hashMode, int memorySize, int iterations, int hashSize)
        {
            if (_sodiumInit < 0)
            {
                throw new ApplicationException($"libsodion init error: {_sodiumInit}");
            }

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
            var result = NativeMethods.crypto_pwhash(hash, (ulong)HashSize, buffer, (ulong)buffer.Length, salt, (ulong)Iterations, (uint)MemorySize * 1024, GetAlg());

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

            var result = NativeMethods.crypto_pwhash_str_verify(hash, buffer, (ulong)buffer.Length);

            return result == 0;
        }

        private int GetAlg()
        {
            return this.HashMode switch
            {
                Argon2Mode.Argon2i => NativeMethods.crypto_pwhash_argon2i_ALG_ARGON2I13,
                Argon2Mode.Argon2id => NativeMethods.crypto_pwhash_argon2id_ALG_ARGON2ID13,
                _ => throw new NotSupportedException($"Argon2 mode {this.HashMode} is currently not supported!"),
            };
        }
    }
}
