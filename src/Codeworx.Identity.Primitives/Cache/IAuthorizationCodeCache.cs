using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IAuthorizationCodeCache
    {
        Task<IdentityData> GetAsync(string authorizationCode);

        Task SetAsync(string authorizationCode, IdentityData payload, TimeSpan timeout);
    }
}