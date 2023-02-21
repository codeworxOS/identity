using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class DefaultSigningKeyProvider : IDefaultSigningKeyProvider, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private AsymmetricSecurityKey _key;
        private AsymmetricAlgorithm _keyAlgorithm;
        private HashAlgorithm _hashAlgorithm;
        private IdentityOptions _options;

        public DefaultSigningKeyProvider(IOptionsMonitor<IdentityOptions> monitor)
        {
            _subscription = monitor.OnChange(p =>
            {
                _options = p;
                LoadKey();
            });
            _options = monitor.CurrentValue;

            LoadKey();

            ////CertificateRequest request = new CertificateRequest(
            ////    "CN=Self-Signed ECDSA",
            ////    key,
            ////    HashAlgorithmName.SHA256);

            ////var request = new CertificateRequest("CN=Whatever", ecd, HashAlgorithmName.SHA384);
            ////var selfSigned = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(3560));
            ////var a = selfSigned.GetECDsaPrivateKey();
            ////var b = selfSigned.GetECDsaPublicKey();

            ////var pubKey = x.GetECDsaPublicKey();
            ////var privateKey = x.GetECDsaPrivateKey();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public HashAlgorithm GetHashAlgorithm()
        {
            return _hashAlgorithm;
        }

        public SecurityKey GetKey()
        {
            return _key;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                    _keyAlgorithm.Dispose();
                    _hashAlgorithm.Dispose();
                }

                _disposedValue = true;
            }
        }

        private static AsymmetricSecurityKey CreateKeyFromAlgorithm(AsymmetricAlgorithm keyAlgorithm)
        {
            if (keyAlgorithm is ECDsa ecd)
            {
                return new ECDsaSecurityKey(ecd);
            }
            else if (keyAlgorithm is RSA rsa)
            {
                return new RsaSecurityKey(rsa);
            }

            throw new ArgumentException("Unsupported key algorithm", nameof(keyAlgorithm));
        }

        private X509Certificate2 LoadCert(SigningOptions options)
        {
            var store = new X509Store(options.Name, options.Location);
            store.Open(OpenFlags.ReadOnly);

            var searchResult = store.Certificates.Find(options.FindBy, options.Search, false);

            var now = DateTime.Now;

            var cert = searchResult.OfType<X509Certificate2>()
                                    .Where(p => p.NotBefore <= now && p.NotAfter > now)
                                    .OrderByDescending(p => p.NotAfter)
                                    .FirstOrDefault();

            if (cert == null)
            {
                throw new CertificateNotFoundException(options);
            }

            return cert;
        }

        private void LoadKey()
        {
            _keyAlgorithm?.Dispose();

            var options = _options.Signing;

            switch (options.Source)
            {
                case KeySource.TemporaryInMemory:
                    _keyAlgorithm = ECDsa.Create(ECCurve.NamedCurves.nistP384);
                    _key = CreateKeyFromAlgorithm(_keyAlgorithm);
                    _hashAlgorithm = CreateHashAlgorithmFromKey(_key);
                    break;
                case KeySource.Store:
                    var cert = LoadCert(options);

                    _keyAlgorithm = cert.GetECDsaPrivateKey();

                    if (_keyAlgorithm == null)
                    {
                        _keyAlgorithm = cert.GetRSAPrivateKey();
                    }

                    if (_keyAlgorithm == null)
                    {
                        throw new NotSupportedException("Unsupported Certificate type!");
                    }

                    _key = CreateKeyFromAlgorithm(_keyAlgorithm);
                    _hashAlgorithm = CreateHashAlgorithmFromCert(cert);

                    break;
                default:
                    throw new NotSupportedException($"The signing source {options.Source} is not supported!");
            }
        }

        private HashAlgorithm CreateHashAlgorithmFromKey(AsymmetricSecurityKey key)
        {
            switch (key.KeySize)
            {
                case 256:
                    return SHA256.Create();
                case 384:
                    return SHA384.Create();
                case 512:
                    return SHA512.Create();
                default:
                    break;
            }

            throw new NotSupportedException("Unsupported signing key.");
        }

        private HashAlgorithm CreateHashAlgorithmFromCert(X509Certificate2 cert)
        {
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