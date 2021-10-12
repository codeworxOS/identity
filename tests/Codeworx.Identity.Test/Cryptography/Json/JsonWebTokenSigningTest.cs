using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Cryptography.Json
{
    public class JsonWebTokenSigningTest
    {
        private static ECDsa LoadPrivateKey(string privateKey)
        {
            var ecDsaCng = new ECDsaCng(CngKey.Import(Convert.FromBase64String(privateKey), CngKeyBlobFormat.EccPrivateBlob));
            ecDsaCng.HashAlgorithm = CngAlgorithm.ECDsaP256;
            return ecDsaCng;
        }

        private static ECDsa LoadPublicKey(string publicKey)
        {
            var ecDsaCng = new ECDsaCng(CngKey.Import(Convert.FromBase64String(publicKey), CngKeyBlobFormat.EccPublicBlob));
            ecDsaCng.HashAlgorithm = CngAlgorithm.ECDsaP256;
            return ecDsaCng;
        }

        [Ignore("For local use only")]
        private async Task SimpleRsaSigningKeyTest()
        {
            var test = new JsonWebTokenHandler();

            CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters();
            keyCreationParameters.ExportPolicy = CngExportPolicies.AllowPlaintextExport;

            var _cngKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null, keyCreationParameters);
            var _ecd = new ECDsaCng(_cngKey);
            _ecd.HashAlgorithm = CngAlgorithm.Sha256;
            var _key = new ECDsaSecurityKey(_ecd);

            var _token = test.CreateToken("{ sub: 'abc' }", new SigningCredentials(_key, "ES384"));

            var provider = new JwtProvider(null);

            //var pk = "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgevZzL1gdAFr88hb2OF/2NxApJCzGCEDdfSp6VQO30hyhRANCAAQRWz+jn65BtOMvdyHKcvjBeBSDZH2r1RTwjmYSi9R/zpBnuQ4EiMnCqfMPWiZqB4QdbAd0E7oH50VpuZ1P087G";

            //var test2 = new X509Certificate2(Convert.FromBase64String(pk));

            //var key = LoadPrivateKey(pk);

            //var ecdsa = new ECDsaSecurityKey(key);

            //var token = await provider.CreateAsync(null, DateTime.Now.AddHours(1));
            X509Certificate2 x = new X509Certificate2(@"c:\temp\es256\ecdas.crt");

            X509Certificate2 y = new X509Certificate2(@"c:\temp\es256\ecdas.pfx", "testchen", X509KeyStorageFlags.Exportable);

            var buffer = y.Export(X509ContentType.Pkcs12);
            var result = Convert.ToBase64String(buffer);

            var z = new X509Certificate2(buffer);

            var ecd = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            var request = new CertificateRequest("CN=Whatever", ecd, HashAlgorithmName.SHA384);
            var selfSigned = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(3560));
            var a = selfSigned.GetECDsaPrivateKey();
            var b = selfSigned.GetECDsaPublicKey();

            var pubKey = x.GetECDsaPublicKey();
            var privateKey = x.GetECDsaPrivateKey();

            var keyBlob = Convert.FromBase64String("MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgrfc4piLYUA18vHPMuANhHB9Nhx4WepnP7edC7OjVjGahRANCAAS6ULl5agfOCzjMPVycyhLTPlhh7ZGdVWgLfqxwKH/b+lU8zbPYI6IbdQM4E6PJH1RI11bhEwgBcAr66UKnD5Wa");

            var key = CngKey.Import(keyBlob, CngKeyBlobFormat.Pkcs8PrivateBlob);
            var blobPrivateKey = new ECDsaCng(key);

            pubKey = y.GetECDsaPublicKey();
            privateKey = y.GetECDsaPrivateKey();

            var token = test.CreateToken("{ sub: 'abc' }", new SigningCredentials(new ECDsaSecurityKey(blobPrivateKey), "ES256"));
        }
    }
}