using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Login
{
    public interface IPasswordPolicyProvider
    {
        Task<IStringPolicy> GetPolicyAsync();
    }
}
