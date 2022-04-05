using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface ILoginPolicyProvider
    {
        Task<IStringPolicy> GetPolicyAsync();
    }
}
