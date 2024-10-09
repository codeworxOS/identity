using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningKeyProvider
    {
        [Obsolete("The interface IDefaultSigningKeyProvider is no longer in use. Use IDefaultSigningDataProvider instead.", true)]
        SecurityKey GetKey();

        [Obsolete("The interface IDefaultSigningKeyProvider is no longer in use. Use IDefaultSigningDataProvider instead.", true)]
        HashAlgorithm GetHashAlgorithm();
    }
}