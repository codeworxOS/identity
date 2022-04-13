using System.Security.Cryptography;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IJwkInformationSerializer
    {
        bool Supports(SecurityKey key);

        KeyParameter SerializeKeyToJsonWebKey(SecurityKey key, string keyId);

        string GetAlgorithm(SecurityKey key, HashAlgorithm hashAlgorithm);
    }
}
