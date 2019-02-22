using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationCodeGenerator
    {
        Task<IAuthorizationCodeGenerationResult> GenerateCode(AuthorizationRequest request, string userIdentifier);
    }
}
