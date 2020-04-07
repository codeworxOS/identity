using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationFlowService<TRequest>
        where TRequest : AuthorizationRequest
    {
        string[] SupportedResponseTypes { get; }

        bool IsSupported(string responseType);

        Task<IAuthorizationResult> AuthorizeRequest(TRequest request, ClaimsIdentity user);
    }
}