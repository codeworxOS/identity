using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class OperationFilterNode : BooleanFilterNode
    {
        public OperationFilterNode(PathFilterNode path, FilterOperator op, string value)
        {
            Path = path;
            Op = op;
            Value = value;
        }

        public PathFilterNode Path { get; }

        public FilterOperator Op { get; }

        public string Value { get; }

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

            throw new NotSupportedException($"Filter for {this.Path} not supported!");
        }

        public override bool Evaluate(JsonObject json)
        {
            var result = Path.Evaluate(json);

            if (result != null)
            {
                return result.ToString() == this.Value;
            }

            return false;
        }
    }
}
