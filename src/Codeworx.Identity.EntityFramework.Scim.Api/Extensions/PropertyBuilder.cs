using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public class PropertyBuilder<TResource>
        where TResource : ISchemaResource, new()
    {
        private IServiceCollection _services;

        public PropertyBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public PropertyBuilder<TResource> AddUserProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<User, TData>> entityExpression)
        {
            var body = entityExpression.Body;
            if (body is MemberExpression me)
            {
                return this.AddUserProperty(resourceExpression, me.Member.Name);
            }

            throw new NotSupportedException();
        }

        public PropertyBuilder<TResource> AddUserProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, string entityPropertyName)
        {
            _services.AddSingleton<IUserSchemaProperty>(new UserSchemaProperty<TResource, TData>(resourceExpression, entityPropertyName));

            return this;
        }

        public PropertyBuilder<TResource> AddGroupProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<Group, TData>> entityExpression)
        {
            var body = entityExpression.Body;
            if (body is MemberExpression me)
            {
                return this.AddGroupProperty(resourceExpression, me.Member.Name);
            }

            throw new NotSupportedException();
        }

        public PropertyBuilder<TResource> AddGroupProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, string entityPropertyName)
        {
            _services.AddSingleton<IGroupSchemaProperty>(new GroupSchemaProperty<TResource, TData>(resourceExpression, entityPropertyName));

            return this;
        }
    }
}
