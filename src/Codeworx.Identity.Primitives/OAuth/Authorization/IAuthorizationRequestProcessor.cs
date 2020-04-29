using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public interface IAuthorizationRequestProcessor
    {
        Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request);
    }
}
