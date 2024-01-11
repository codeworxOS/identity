using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class OperationFilterNode : FilterNode
    {
        public OperationFilterNode(string[] paths, FilterOperator op, string value, string? schema = null)
        {
            Paths = paths;
            Op = op;
            Value = value;
            Schema = schema;
        }

        public string[] Paths { get; }

        public FilterOperator Op { get; }

        public string Value { get; }

        public string? Schema { get; }

        public override Expression<Func<ScimEntity<TEntity>, bool>> Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            foreach (var item in mappings)
            {
                var filter = item.GetFilter(this);
                if (filter != null)
                {
                    return filter;
                }
            }

            throw new NotSupportedException($"Filter for {string.Join(".", this.Paths)} not supported!");
        }

        public override bool Evaluate(JsonObject json)
        {
            for (int i = 0; i < Paths.Length; i++)
            {
                if (i < Paths.Length - 1)
                {
                    json = json[Paths[i]]!.AsObject();
                }
                else
                {
                    return json[Paths[i]]!.ToString() == this.Value;
                }
            }

            return false;
        }
    }
}
