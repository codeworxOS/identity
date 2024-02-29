using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cryptography
{
    public interface ICertificateStore
    {
        Task<X509Certificate2> LoadAsync(string key, CancellationToken token);
    }
}
