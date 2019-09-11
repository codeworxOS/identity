using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IRequestValidator<in TRequest, TResult>
    {
        Task<IValidationResult<TResult>> IsValid(TRequest request);
    }
}
