using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Cache
{
    public interface IStateLookupCache
    {
        Task<StateLookupItem> GetAsync(string state);

        Task SetAsync(string state, StateLookupItem value);
    }
}
