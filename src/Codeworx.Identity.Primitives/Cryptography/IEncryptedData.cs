namespace Codeworx.Identity.Cryptography
{
    public interface IEncryptedData
    {
        byte[] Key { get; }

        byte[] Data { get; }
    }
}
