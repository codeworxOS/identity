using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public class SigningData : IDisposable
    {
        private readonly AsymmetricAlgorithm _keyAlgorithm;
        private bool _disposedValue;

        public SigningData(SecurityKey key, AsymmetricAlgorithm keyAlgorithm, HashAlgorithm hash)
        {
            _keyAlgorithm = keyAlgorithm;
            Key = key;
            Hash = hash;

            string algorithm = null;

            switch (Key)
            {
                case ECDsaSecurityKey ecd:
                    algorithm = $"ES{hash.HashSize}";
                    break;

                case RsaSecurityKey rsa:
                case X509SecurityKey x509:
                    algorithm = $"RS{hash.HashSize}";
                    break;

                default:
                    throw new NotSupportedException("provided Signing Key is not supported!");
            }

            Credentials = new SigningCredentials(key, algorithm);
        }

        public SigningCredentials Credentials { get; }

        public HashAlgorithm Hash { get; }

        public SecurityKey Key { get; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Hash.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
