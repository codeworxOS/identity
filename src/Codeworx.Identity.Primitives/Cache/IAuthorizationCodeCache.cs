using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IAuthorizationCodeCache
    {
        Task<IDictionary<string, string>> GetAsync(string authorizationCode);

        Task SetAsync(string authorizationCode, IDictionary<string, string> payload, TimeSpan timeout);
    }
}