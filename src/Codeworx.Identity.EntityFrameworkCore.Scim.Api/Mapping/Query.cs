using System;
using System.Linq;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public static class Query
    {
        public static Guid ProviderId => throw new NotSupportedException("Should be replaced before execute!");

        public static IQueryable<TEntity> Set<TEntity>()
        {
            throw new NotSupportedException("Should be replaced before execute!");
        }
    }
}
