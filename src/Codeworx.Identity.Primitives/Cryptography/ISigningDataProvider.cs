using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public interface ISigningDataProvider
    {
        Task<SigningData> GetSigningDataAsync(string key, CancellationToken token);
    }
}
