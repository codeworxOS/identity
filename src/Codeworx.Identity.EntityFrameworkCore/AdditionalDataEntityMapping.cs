using System;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class AdditionalDataEntityMapping<TTarget, TEntity> : IAdditionalDataEntityMapping
    {
        public AdditionalDataEntityMapping()
        {
            Target = typeof(TTarget);
            Entity = typeof(TEntity);
        }

        public Type Target { get; }

        public Type Entity { get; }
    }
}
