using System.Security.Cryptography;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class RsaSigningKeyProvider : IDefaultSigningKeyProvider
    {
        private const string Identifier = "E134BE45-0F2F-4D6D-B331-61680701AC3B";

        private readonly SecurityKey _key;
        private RSACryptoServiceProvider _rsa;

        public RsaSigningKeyProvider()
        {
            _rsa = new RSACryptoServiceProvider(2048);

            _key = new RsaSecurityKey(_rsa);
        }

        public string Algorithm { get; } = "RS256";

        public string KeyId => Identifier;

        public SecurityKey GetKey()
        {
            return _key;
        }

        public KeyParameter GetKeyParameter()
        {
            var parameter = _rsa.ExportParameters(false);

            var n = Base64UrlEncoder.Encode(parameter.Modulus);
            var e = Base64UrlEncoder.Encode(parameter.Exponent);

            return new RsaKeyParameter(Identifier, KeyUse.Signature, e, n);
        }
    }
}