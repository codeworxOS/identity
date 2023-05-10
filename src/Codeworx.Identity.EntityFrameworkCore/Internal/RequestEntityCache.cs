using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.EntityFrameworkCore.Internal
{
    internal class RequestEntityCache : IRequestEntityCache
    {
        private readonly ConcurrentDictionary<Guid, IEnumerable<TenantInfo>> _tenantInfoCache;
        private IEnumerable<TenantInfo> _allTenantInfos;

        public RequestEntityCache()
        {
            _tenantInfoCache = new ConcurrentDictionary<Guid, IEnumerable<TenantInfo>>();
        }

        public async Task<IEnumerable<TenantInfo>> GetTenantInfos(Guid rightHolderId, Func<Guid, Task<IEnumerable<TenantInfo>>> factory)
        {
            if (_tenantInfoCache.TryGetValue(rightHolderId, out var value))
            {
                return value;
            }

            var data = await factory(rightHolderId);

            return _tenantInfoCache.AddOrUpdate(rightHolderId, data, (p, q) => data);
        }

        public async Task<IEnumerable<TenantInfo>> GetTenantInfos(Func<Task<IEnumerable<TenantInfo>>> factory)
        {
            if (_allTenantInfos == null)
            {
                var data = await factory();
                _allTenantInfos = data;
            }

            return _allTenantInfos;
        }
    }
}
