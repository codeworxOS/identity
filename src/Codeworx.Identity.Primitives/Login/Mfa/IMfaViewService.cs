using System.Threading.Tasks;

namespace Codeworx.Identity.Login.Mfa
{
    public interface IMfaViewService
    {
        Task<MfaLoginResponse> ProcessLoginAsync(MfaLoginRequest request);
    }
}
