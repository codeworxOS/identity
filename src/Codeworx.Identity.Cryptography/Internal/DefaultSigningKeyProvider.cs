using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class DefaultSigningKeyProvider : IDefaultSigningKeyProvider
    {
        private readonly ECDsaSecurityKey _key;
        private ECDsa _ecd;

        public DefaultSigningKeyProvider()
        {
            _ecd = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            _key = new ECDsaSecurityKey(_ecd);

            ////var request = new CertificateRequest("CN=Whatever", ecd, HashAlgorithmName.SHA384);
            ////var selfSigned = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(3560));
            ////var a = selfSigned.GetECDsaPrivateKey();
            ////var b = selfSigned.GetECDsaPublicKey();

            ////var pubKey = x.GetECDsaPublicKey();
            ////var privateKey = x.GetECDsaPrivateKey();
        }

        public SecurityKey GetKey()
        {
            return _key;
        }

        public HashAlgorithm GetHashAlgorithm()
        {
            return SHA384.Create();
        }
    }
}