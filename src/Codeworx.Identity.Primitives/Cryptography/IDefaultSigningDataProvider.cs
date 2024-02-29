using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public interface IDefaultSigningDataProvider
    {
        Task<SigningData> GetSigningDataAsync(CancellationToken token);
    }
}