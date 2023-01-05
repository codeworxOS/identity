using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IAuthorizationCodeCache
    {
        Task<IdentityData> GetAsync(string authorizationCode, CancellationToken token = default);

        Task<string> SetAsync(IdentityData payload, TimeSpan validFor, CancellationToken token = default);
    }
}