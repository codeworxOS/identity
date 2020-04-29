using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationFlowService
    {
        string[] SupportedResponseTypes { get; }

        bool IsSupported(string responseType);

        Task<IAuthorizationResult> AuthorizeRequest(IAuthorizationParameters parameters);
    }
}