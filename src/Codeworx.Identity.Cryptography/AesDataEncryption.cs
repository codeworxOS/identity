using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public class AesDataEncryption : ISymmetricDataEncryption
    {
        public async Task<byte[]> DecryptAsync(byte[] data, byte[] key)
        {
            var result = new List<byte>();

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                var keySize = aes.Key.Length;

                aes.Key = key.Take(keySize).ToArray();
                aes.IV = key.Skip(keySize).ToArray();

                using (var memory = new MemoryStream(data))
                using (var cryptoStream = new CryptoStream(
                         memory,
                         aes.CreateDecryptor(),
                         CryptoStreamMode.Read))
                {
                    var buffer = new byte[512];
                    int read = 0;
                    do
                    {
                        read = await cryptoStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            result.AddRange(buffer.Take(read));
                        }
                    }
                    while (read > 0);
                }

                return result.ToArray();
            }
        }

        public async Task<IEncryptedData> EncryptAsync(byte[] data, byte[] key = null)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;

                if (key != null)
                {
                    aes.Key = key.Take(aes.Key.Length).ToArray();
                    aes.IV = key.Skip(aes.Key.Length).ToArray();
                }
                else
                {
                    aes.GenerateKey();
                    aes.GenerateIV();
                }

                using (var memory = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(
                        memory,
                        aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(data, 0, data.Length);
                    }

                    var aesKey = new byte[aes.Key.Length + aes.IV.Length];
                    Buffer.BlockCopy(aes.Key, 0, aesKey, 0, aes.Key.Length);
                    Buffer.BlockCopy(aes.IV, 0, aesKey, aes.Key.Length, aes.IV.Length);

                    var result = new AesEncryptedData(aesKey, memory.ToArray());

                    return result;
                }
            }
        }
    }
}
