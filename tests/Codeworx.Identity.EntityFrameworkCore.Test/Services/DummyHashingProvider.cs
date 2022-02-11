namespace Codeworx.Identity.Test.Services
{
    using Codeworx.Identity.Cryptography;

    public class DummyHashingProvider : IHashingProvider
    {
        public string Create(string plaintext)
        {
            return ToHash(plaintext);
        }

        private static string ToHash(string plaintext)
        {
            return $"Hash:{plaintext}";
        }

        public bool Validate(string plaintext, string hashValue)
        {
            return ToHash(plaintext) == hashValue;
        }
    }
}