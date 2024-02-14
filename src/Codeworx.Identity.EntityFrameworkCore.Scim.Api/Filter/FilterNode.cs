using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public abstract class FilterNode
    {
        public IEnumerable<FilterNode> Flatten()
        {
            yield return this;

            foreach (var child in GetChildren())
            {
                yield return child;
            }
        }

        protected abstract IEnumerable<FilterNode> GetChildren();
    }
}
