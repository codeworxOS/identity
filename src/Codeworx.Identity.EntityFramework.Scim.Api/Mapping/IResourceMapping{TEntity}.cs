using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public interface IResourceMapping<TEntity>
        where TEntity : class
    {
        LambdaExpression ResourceExpression { get; }

        LambdaExpression EntityExpression { get; }

        Task ToDatabaseAsync(DbContext db, TEntity entity, ISchemaResource resource);
    }
}
