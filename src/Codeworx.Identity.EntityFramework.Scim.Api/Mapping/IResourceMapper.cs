using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public interface IResourceMapper<TEntity>
        where TEntity : class
    {
        IQueryable<Dictionary<string, IScimResource>> GetResourceQuery(DbContext db, IQueryable<ScimEntity<TEntity>> baseQuery, QueryParameter parameter);

        Task ToDatabaseAsync(DbContext db, TEntity entity, IEnumerable<ISchemaResource> resources, Guid providerId);

        IAsyncEnumerable<SchemaDataResource> GetSchemasAsync(DbContext db);
    }
}
