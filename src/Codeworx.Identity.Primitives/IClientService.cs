using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IClientService
    {
        Task<IClientRegistration> GetById(string clientIdentifier);
    }
}
