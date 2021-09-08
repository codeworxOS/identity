using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IProfileViewTemplate
    {
        Task<string> GetProfileViewTemplate();
    }
}
