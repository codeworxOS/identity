using System;
using System.Security.Cryptography;
using System.Text;

namespace Codeworx.Identity.Cryptography
{
    public class DefaultSecretGenerator : ISecretGenerator
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public string Create(int length)
        {
            byte[] data = new byte[4 * length];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % _chars.Length;

                result.Append(_chars[idx]);
            }

            return result.ToString();
        }
    }
}
