using System.Security.Cryptography;
using Codeworx.Identity.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Web.Test
{
    public class RsaDefaultSigningKeyProvider : IDefaultSigningKeyProvider
    {
        private readonly SecurityKey _key;

        public RsaDefaultSigningKeyProvider()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            _key = new RsaSecurityKey(rsa);
        }

        public SecurityKey GetKey()
        {
            return _key;
        }

        public HashAlgorithm GetHashAlgorithm()
        {
            return SHA512.Create();
        }
    }
}