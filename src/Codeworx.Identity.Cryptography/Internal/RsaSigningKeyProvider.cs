using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class RsaSigningKeyProvider : IDefaultSigningKeyProvider
    {
        private readonly SecurityKey _key;
        private RSACryptoServiceProvider _rsa;

        public RsaSigningKeyProvider()
        {
            _rsa = new RSACryptoServiceProvider(2048);

            _key = new RsaSecurityKey(_rsa);
        }

        public SecurityKey GetKey()
        {
            return _key;
        }
    }
}