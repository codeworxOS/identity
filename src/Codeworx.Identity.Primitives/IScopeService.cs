using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IScopeService
    {
        Task<IEnumerable<IScope>> GetScopes();
    }
}
