using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Cache
{
    public interface IExternalTokenCache
    {
        Task<ExternalTokenData> GetAsync(string key, TimeSpan extend);

        Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor);

        Task UpdateAsync(string key, ExternalTokenData value, TimeSpan validFor);
    }
}
