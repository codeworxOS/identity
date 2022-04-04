using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public interface IConfirmationService
    {
        Task RequireConfirmationAsync(IUser user, CancellationToken token = default);

        Task ConfirmAsync(IUser user, string code, CancellationToken token = default);
    }
}
