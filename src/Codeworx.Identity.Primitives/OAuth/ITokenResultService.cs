using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenResultService
    {
        string SupportedGrantType { get; }

        Task<string> CreateAccessToken(IDictionary<string, string> cacheData);
    }
}