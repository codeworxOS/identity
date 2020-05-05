using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenResultService
    {
        string SupportedGrantType { get; }

        Task<string> CreateAccessToken(IdentityData data, TimeSpan expiresIn);

        Task<string> CreateIdToken(IdentityData data, TimeSpan expiresIn);
    }
}