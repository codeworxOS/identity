using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningKeyProvider
    {
        string Algorithm { get; }

        string KeyId { get; }

        SecurityKey GetKey();

        KeyParameter GetKeyParameter();
    }
}