﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public abstract class ValueFilterNode : FilterNode
    {
        public LambdaExpression ToExpression<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
            where TEntity : class
        {
            return Convert<TEntity>(mappings);
        }

        public abstract JsonNode? Evaluate(JsonObject json);

        public abstract void SetValue(JsonObject json, JsonNode value);

        public abstract LambdaExpression Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
            where TEntity : class;
    }
}
