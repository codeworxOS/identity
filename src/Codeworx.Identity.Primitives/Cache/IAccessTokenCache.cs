using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IAccessTokenCache
    {
        Task<IdentityData> GetAsync(string tokenKey);

        Task<string> SetAsync(string accessToken, IdentityData payload, TimeSpan validFor);
    }
}