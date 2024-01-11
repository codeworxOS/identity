using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityScimPropertyBuilderExtensions
    {
        public static PropertyBuilder<TResource, TEntity> AddExternalIdProperty<TResource, TEntity>(this PropertyBuilder<TResource, TEntity> propertyBuilder, Expression<Func<TResource, string>> resourceExpression)
            where TResource : IScimResource
            where TEntity : RightHolder
        {
            propertyBuilder.AddMapping(new ExternalIdResourceMapping<TEntity, TResource>(resourceExpression));
            return propertyBuilder;
        }

        public static PropertyBuilder<GroupResource, Group> AddMembersProperty(this PropertyBuilder<GroupResource, Group> propertyBuilder)
        {
            propertyBuilder.AddMapping(new MembersResourceMapping());

            return propertyBuilder;
        }
    }
}
