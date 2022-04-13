using System;
using System.Security.Cryptography;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class RsaJwkSerializer : IJwkInformationSerializer
    {
        public bool Supports(SecurityKey key) => key is RsaSecurityKey;

        public KeyParameter SerializeKeyToJsonWebKey(SecurityKey key, string keyId)
        {
            if (key is RsaSecurityKey rsaKey)
            {
                var parameters = rsaKey.Rsa.ExportParameters(false);

                return new RsaKeyParameter(keyId, Constants.KeyUse.Signature, Base64UrlEncoder.Encode(parameters.Exponent), Base64UrlEncoder.Encode(parameters.Modulus));
            }

            throw new NotSupportedException("Key type not supported!");
        }

        public string GetAlgorithm(SecurityKey key, HashAlgorithm hashAlgorithm)
        {
            if (key is RsaSecurityKey rsaKey)
            {
                return $"RS{hashAlgorithm.HashSize}";
            }

            throw new NotSupportedException("Key type not supported!");
        }
    }
}
