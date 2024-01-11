using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class ArrayFilterNode : ValueFilterNode
    {
        public ArrayFilterNode(ValueFilterNode path, BooleanFilterNode filter, string? member = null)
        {
            Path = path;
            Filter = filter;
            Member = member;
        }

        public ValueFilterNode Path { get; }

        public BooleanFilterNode Filter { get; }

        public string? Member { get; }

        public string? Schema { get; }

        public IEnumerable<JsonNode> GetItems(JsonObject json)
        {
            JsonArray? array = null;

            array = this.Path.Evaluate(json) as JsonArray;

            if (array == null)
            {
                throw new NotSupportedException("Invalid Path!");
            }

            foreach (var row in array)
            {
                if (Filter.Evaluate(row!.AsObject()))
                {
                    yield return row;
                }
            }
        }

        public override object? Evaluate(JsonObject json)
        {
            if (Member != null)
            {
                throw new NotSupportedException("Invalid path!");
            }

            foreach (var item in GetItems(json))
            {
                if (Member != null)
                {
                    if (item.AsObject().TryGetPropertyValue(Member, out var value))
                    {
                        return value;
                    }
                }

                return item;
            }

            return null;
        }

        public override LambdaExpression Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            throw new NotImplementedException();
        }
    }
}
