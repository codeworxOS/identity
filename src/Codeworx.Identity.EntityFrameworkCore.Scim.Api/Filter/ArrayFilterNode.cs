using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<JsonNode> GetItems(JsonObject json, bool createIfNotExists)
        {
            JsonArray? array = null;

            array = this.Path.Evaluate(json) as JsonArray;

            if (array == null)
            {
                throw new NotSupportedException("Invalid Path!");
            }

            bool found = false;

            foreach (var row in array)
            {
                if (Filter.Evaluate(row!.AsObject()))
                {
                    found = true;
                    yield return row;
                }
            }

            if (!found && createIfNotExists)
            {
                var compareNodes = Filter.Flatten().OfType<OperationFilterNode>().ToList();
                var newItem = new JsonObject();
                array.Add(newItem);

                foreach (var item in compareNodes)
                {
                    item.Path.SetValue(newItem, JsonNode.Parse(item.RawValue)!);
                }

                yield return newItem;
            }
        }

        public override JsonNode? Evaluate(JsonObject json)
        {
            if (Member != null)
            {
                throw new NotSupportedException("Invalid path!");
            }

            foreach (var item in GetItems(json, false))
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

        public override void SetValue(JsonObject json, JsonNode value)
        {
            var array = this.Path.Evaluate(json) as JsonArray;

            if (array != null)
            {
                array.Clear();

                if (value is JsonArray valueArray)
                {
                    foreach (var row in valueArray.ToList())
                    {
                        valueArray.Remove(row);
                        array.Add(row);
                    }
                }
                else
                {
                    array.Add(value);
                }
            }
        }

        protected override IEnumerable<FilterNode> GetChildren()
        {
            return this.Path.Flatten().Concat(this.Filter.Flatten());
        }
    }
}
