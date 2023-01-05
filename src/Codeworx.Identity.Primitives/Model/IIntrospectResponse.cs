using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IIntrospectResponse
    {
        bool IsActive { get; }

        IDictionary<string, object> AdditionalProperties { get; }
    }
}
