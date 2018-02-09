namespace Codeworx.Identity.Cryptography
{
    public interface IHashingProvider
    {
        byte[] CrateSalt();

        byte[] Hash(string text, byte[] salt);
    }
}