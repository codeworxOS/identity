using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningKeyProvider
    {
        string Algorithm { get; }

        SecurityKey GetKey();

        KeyParameter GetKeyParameter();
    }
}