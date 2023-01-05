using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public interface IIntrospectionService
    {
        Task<IIntrospectResponse> ProcessAsync(IntrospectRequest request, CancellationToken token = default);
    }
}
