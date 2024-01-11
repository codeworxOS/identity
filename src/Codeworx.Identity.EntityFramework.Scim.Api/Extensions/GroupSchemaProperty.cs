using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

public class GroupSchemaProperty<TResource, TProperty> : IGroupSchemaProperty
    where TResource : ISchemaResource, new()
{
    private Func<TResource, TProperty?> _compiledResourceExpression;
    private Delegate _compiledAssignExpression;

    public GroupSchemaProperty(Expression<Func<TResource, TProperty>> resourceExpression, string entityPropertyName)
    {
        EntityPropertyName = entityPropertyName;

        _compiledResourceExpression = resourceExpression.Compile();

        var inputParam = Expression.Parameter(resourceExpression.Body.Type);
        _compiledAssignExpression = Expression.Lambda(Expression.Assign(resourceExpression.Body, inputParam), resourceExpression.Parameters[0], inputParam).Compile();
    }

    public Type ResourceType => typeof(TResource);

    public string EntityPropertyName { get; }

    public object? GetResourceValue(object resource)
    {
        if (resource is not TResource res)
        {
            throw new NotSupportedException();
        }

        return _compiledResourceExpression(res);
    }

    public void SetResourceValue(object resource, object? value)
    {
        if (resource is not TResource res)
        {
            throw new NotSupportedException();
        }

        _compiledAssignExpression.DynamicInvoke(res, value);
    }
}
