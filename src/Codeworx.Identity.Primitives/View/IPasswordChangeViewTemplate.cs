using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IPasswordChangeViewTemplate
    {
        Task<string> GetPasswordChangeTemplate();
    }
}
