namespace Codeworx.Identity.Cryptography
{
    public class AesEncryptedData : IEncryptedData
    {
        public AesEncryptedData(byte[] key, byte[] data)
        {
            Key = key;
            Data = data;
        }

        public byte[] Data { get; }

        public byte[] Key { get; }
    }
}
