using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public interface IResourceMapping<TEntity>
        where TEntity : class
    {
        string ResourcePath { get; }

        LambdaExpression ResourceExpression { get; }

        LambdaExpression EntityExpression { get; }

        Task ToDatabaseAsync(DbContext db, TEntity entity, ISchemaResource resource, Guid providerId);

        IAsyncEnumerable<SchemaInfo> GetSchemaAttributesAsync(DbContext db);

        Expression<Func<ScimEntity<TEntity>, bool>>? GetFilter(OperationFilterNode operationFilterNode);
    }
}
