using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class SigningDataProvider : ISigningDataProvider
    {
        private readonly ICertificateStore _certificateStore;

        public SigningDataProvider(ICertificateStore certificateStore)
        {
            _certificateStore = certificateStore;
        }

        public async Task<SigningData> GetSigningDataAsync(string key, CancellationToken token)
        {
            using (var cert = await _certificateStore.LoadAsync(key, token))
            {
                AsymmetricAlgorithm keyAlgorithm = cert.GetECDsaPrivateKey();

                if (keyAlgorithm == null)
                {
                    keyAlgorithm = cert.GetRSAPrivateKey();
                }

                if (keyAlgorithm == null)
                {
                    throw new NotSupportedException("Unsupported Certificate type!");
                }

                var securityKey = CreateKeyFromAlgorithm(keyAlgorithm, cert);
                var hashAlgorithm = CreateHashAlgorithmFromCert(cert, securityKey);

                return new SigningData(securityKey, keyAlgorithm, hashAlgorithm);
            }
        }

        private static AsymmetricSecurityKey CreateKeyFromAlgorithm(AsymmetricAlgorithm keyAlgorithm, X509Certificate2 cert)
        {
            if (keyAlgorithm is ECDsa ecd)
            {
                return new ECDsaSecurityKey(ecd);
            }
            else if (keyAlgorithm is RSA rsa)
            {
                return new X509SecurityKey(new X509Certificate2(cert));
            }

            throw new ArgumentException("Unsupported key algorithm", nameof(keyAlgorithm));
        }

        private HashAlgorithm CreateHashAlgorithmFromKey(ECDsaSecurityKey key)
        {
            switch (key.KeySize)
            {
                case 256:
                    return SHA256.Create();
                case 384:
                    return SHA384.Create();
                case 521:
                    return SHA512.Create();
                default:
                    break;
            }

            throw new NotSupportedException("Unsupported signing key.");
        }

        private HashAlgorithm CreateHashAlgorithmFromCert(X509Certificate2 cert, AsymmetricSecurityKey key)
        {
            if (key is ECDsaSecurityKey ecd)
            {
                return CreateHashAlgorithmFromKey(ecd);
            }

            switch (cert.SignatureAlgorithm.FriendlyName)
            {
                case "sha256RSA":
                    return SHA256.Create();
                case "sha384RSA":
                    return SHA384.Create();
                case "sha512RSA":
                    return SHA512.Create();
                default:
                    break;
            }

            throw new NotSupportedException("Unsupported signing key.");
        }
    }
}
