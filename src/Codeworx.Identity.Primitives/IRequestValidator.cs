using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IRequestValidator<in TRequest>
    {
        Task ValidateAsync(TRequest request);
    }
}
