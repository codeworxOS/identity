using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public interface ISymmetricDataEncryption
    {
        Task<IEncryptedData> EncryptAsync(byte[] data, byte[] key = null);

        Task<byte[]> DecryptAsync(byte[] data, byte[] key);
    }
}
