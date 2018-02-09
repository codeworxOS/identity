using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IPasswordValidator
    {
        Task<bool> Validate(IUser user, string password);
    }
}