using System.Collections.Generic;

namespace Codeworx.Identity.View
{
    public interface IViewData
    {
        void CopyTo(IDictionary<string, object> target);
    }
}
