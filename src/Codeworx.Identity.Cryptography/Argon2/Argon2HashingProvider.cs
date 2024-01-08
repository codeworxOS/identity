using System.Text;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2HashingProvider : IHashingProvider
    {
        private readonly Argon2Options _options;
        private readonly ISecretGenerator _secretGenerator;

        public Argon2HashingProvider(Argon2Options options, ISecretGenerator secretGenerator)
        {
            _options = options;
            _secretGenerator = secretGenerator;
        }

        public string Create(string plaintext)
        {
            byte[] salt = Encoding.UTF8.GetBytes(_secretGenerator.Create(16));

            var argon2 = new Argon2(_options.Argon2Mode, _options.MemorySize, _options.Iterations, _options.HashLength);

            return argon2.CreateHash(plaintext, salt);
        }

        public bool Validate(string plaintext, string hashValue)
        {
            var argon2 = new Argon2(_options.Argon2Mode, _options.MemorySize, _options.Iterations, _options.HashLength);

            return argon2.Validate(plaintext, hashValue);
        }
    }
}
