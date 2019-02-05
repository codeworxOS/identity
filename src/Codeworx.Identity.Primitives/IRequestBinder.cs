using System.Collections.Generic;

namespace Codeworx.Identity
{
    public interface IRequestBinder<out TRequest>
    {
        TRequest FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query);
    }
}
