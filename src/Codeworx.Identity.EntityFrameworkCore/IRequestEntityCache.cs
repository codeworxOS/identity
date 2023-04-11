using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public interface IRequestEntityCache
    {
        Task<IEnumerable<TenantInfo>> GetTenantInfos(Guid rightHolderId, Func<Guid, Task<IEnumerable<TenantInfo>>> factory);

        Task<IEnumerable<TenantInfo>> GetTenantInfos(Func<Task<IEnumerable<TenantInfo>>> factory);
    }
}
