using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningKeyProvider
    {
        SecurityKey GetKey();
    }
}
