using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Codeworx.Identity.Cryptography.Pbkdf2
{
    public class Pbkdf2Options
    {
        public KeyDerivationPrf HashAlgorithm { get; set; }

        public int Iterations { get; set; }

        public byte OutputLength { get; set; }

        public byte SaltLength { get; set; }
    }
}