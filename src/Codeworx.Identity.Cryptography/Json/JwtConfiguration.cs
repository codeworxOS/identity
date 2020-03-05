using System.Runtime.Serialization;

namespace Codeworx.Identity.Cryptography.Json
{
    [DataContract]
    public class JwtConfiguration
    {
        public JwtConfiguration()
        {
            var token = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        }
    }
}