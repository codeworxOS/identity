using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationCodeGenerator<TRequest>
        where TRequest : AuthorizationRequest
    {
        Task<string> GenerateCode(TRequest request, int length);
    }
}