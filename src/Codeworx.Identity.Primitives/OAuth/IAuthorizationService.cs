using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationService
    {
        Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IdentityData user);
    }
}