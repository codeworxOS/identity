using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResult> AuthorizeRequest(AuthorizationRequest request, string userIdentifier);
    }
}
