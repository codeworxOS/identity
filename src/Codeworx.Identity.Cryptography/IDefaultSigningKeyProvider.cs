using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningKeyProvider
    {
        SecurityKey GetKey();

        HashAlgorithm GetHashAlgorithm();
    }
}