using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Cache
{
    public interface IExternalTokenCache
    {
        Task<ExternalTokenData> GetAsync(string key, CancellationToken token = default);

        Task ExtendAsync(string key, TimeSpan extension, CancellationToken token = default);

        Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor, CancellationToken token = default);

        Task UpdateAsync(string key, ExternalTokenData value, CancellationToken token = default);
    }
}
