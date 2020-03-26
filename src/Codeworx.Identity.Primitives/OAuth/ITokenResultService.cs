using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenResultService
    {
        string SupportedGrantType { get; }

        Task<ITokenResult> ProcessRequest(TokenRequest request);
    }
}