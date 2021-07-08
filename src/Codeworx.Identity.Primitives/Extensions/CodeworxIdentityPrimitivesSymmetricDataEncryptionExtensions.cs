using System;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Internal;

namespace Codeworx.Identity.Cryptography
{
    public static class CodeworxIdentityPrimitivesSymmetricDataEncryptionExtensions
    {
        public static async Task<(string Key, string Data)> EncryptAsync(this ISymmetricDataEncryption encryption, string value, string key = null)
        {
            var keyBuffer = key != null ? Base64UrlEncoding.DecodeBytes(key) : null;
            var dataBuffer = Encoding.UTF8.GetBytes(value);

            var encrypted = await encryption.EncryptAsync(dataBuffer, keyBuffer);

            return (Base64UrlEncoding.Encode(encrypted.Key), Convert.ToBase64String(encrypted.Data));
        }

        public static async Task<string> DecryptAsync(this ISymmetricDataEncryption encryption, string value, string key)
        {
            var keyBuffer = Base64UrlEncoding.DecodeBytes(key);
            var dataBuffer = Convert.FromBase64String(value);

            var result = await encryption.DecryptAsync(dataBuffer, keyBuffer);

            return Encoding.UTF8.GetString(result);
        }
    }
}