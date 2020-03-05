using System.Collections.Generic;

namespace Codeworx.Identity
{
    public interface IRequestBinder<out TRequest, out TError>
    {
        IRequestBindingResult<TRequest, TError> FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query);
    }
}
