using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationFlowService
    {
        string SupportedAuthorizationResponseType { get; }

        Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IUser user, string currentTenantIdentifier);
    }
}
