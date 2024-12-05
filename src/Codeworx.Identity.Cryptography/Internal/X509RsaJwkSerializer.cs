using System;
using System.Security.Cryptography;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class X509RsaJwkSerializer : IJwkInformationSerializer
    {
        public bool Supports(SecurityKey key) => key is X509SecurityKey x509 && x509.PrivateKey is RSA;

        public KeyParameter SerializeKeyToJsonWebKey(SecurityKey key, string keyId)
        {
            if (key is X509SecurityKey x509SecurityKey && x509SecurityKey.PrivateKey is RSA rsa)
            {
                var parameters = rsa.ExportParameters(false);
                return new RsaKeyParameter(keyId, Constants.KeyUse.Signature, Base64UrlEncoder.Encode(parameters.Exponent), Base64UrlEncoder.Encode(parameters.Modulus));
            }

            throw new NotSupportedException("Key type not supported!");
        }

        public string GetAlgorithm(SecurityKey key, HashAlgorithm hashAlgorithm)
        {
            if (key is X509SecurityKey rsaKey && rsaKey.PrivateKey is RSA)
            {
                return $"RS{hashAlgorithm.HashSize}";
            }

            throw new NotSupportedException("Key type not supported!");
        }
    }
}
