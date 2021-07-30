using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IAuthorizationCodeCache
    {
        Task<IdentityData> GetAsync(string authorizationCode);

        Task<string> SetAsync(IdentityData payload, TimeSpan validFor);
    }
}