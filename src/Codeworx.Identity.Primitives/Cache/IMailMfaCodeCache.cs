using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IMailMfaCodeCache
    {
        Task<string> GetAsync(string key, CancellationToken token = default);

        Task<string> CreateAsync(string key, TimeSpan validFor, CancellationToken token = default);
    }
}
