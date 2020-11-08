namespace Codeworx.Identity.Cryptography
{
    public interface IHashingProvider
    {
        string Create(string plaintext);

        bool Validate(string plaintext, string hashValue);
    }
}