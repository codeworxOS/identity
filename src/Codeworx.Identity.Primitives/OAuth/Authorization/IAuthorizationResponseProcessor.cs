using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public interface IAuthorizationResponseProcessor
    {
        Task<IAuthorizationResponseBuilder> ProcessAsync(IAuthorizationParameters parameters, IdentityData data, IAuthorizationResponseBuilder responseBuilder);
    }
}
