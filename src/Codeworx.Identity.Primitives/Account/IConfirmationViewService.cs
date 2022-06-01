using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public interface IConfirmationViewService
    {
        Task<ConfirmationResponse> ProcessAsync(ConfirmationRequest request, CancellationToken token = default);
    }
}
