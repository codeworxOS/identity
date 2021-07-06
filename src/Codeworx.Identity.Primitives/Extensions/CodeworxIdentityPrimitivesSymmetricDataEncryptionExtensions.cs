using System;
using System.Text;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public static class CodeworxIdentityPrimitivesSymmetricDataEncryptionExtensions
    {
        public static async Task<(string Key, string Data)> EncryptAsync(this ISymmetricDataEncryption encryption, string value, string key = null)
        {
            var keyBuffer = key != null ? Convert.FromBase64String(key) : null;
            var dataBuffer = Encoding.UTF8.GetBytes(value);

            var encrypted = await encryption.EncryptAsync(dataBuffer, keyBuffer);

            return (Convert.ToBase64String(encrypted.Key), Convert.ToBase64String(encrypted.Data));
        }

        public static async Task<string> DecryptAsync(this ISymmetricDataEncryption encryption, string value, string key)
        {
            var keyBuffer = Convert.FromBase64String(key);
            var dataBuffer = Convert.FromBase64String(value);

            var result = await encryption.DecryptAsync(dataBuffer, keyBuffer);

            return Encoding.UTF8.GetString(result);
        }
    }
}