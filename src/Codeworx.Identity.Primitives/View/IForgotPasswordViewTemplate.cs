using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IForgotPasswordViewTemplate
    {
        Task<string> GetForgotPasswordTemplate();

        Task<string> GetForgotPasswordCompletedTemplate();
    }
}
