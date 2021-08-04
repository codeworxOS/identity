using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Account
{
    public interface IProfileService
    {
        Task<ProfileResponse> ProcessAsync(ProfileRequest request);

        Task<ProfileLinkResponse> ProcessLinkAsync(ProfileLinkRequest request);
    }
}
