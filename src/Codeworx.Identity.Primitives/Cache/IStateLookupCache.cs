using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IStateLookupCache
    {
        Task<string> GetAsync(string state);

        Task SetAsync(string state, string value);
    }
}
