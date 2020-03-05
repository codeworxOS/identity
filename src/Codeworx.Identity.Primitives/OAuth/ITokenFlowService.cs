using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenFlowService
    {
        string SupportedGrantType { get; }

        Task<ITokenResult> AuthorizeRequest(TokenRequest request);
    }
}
