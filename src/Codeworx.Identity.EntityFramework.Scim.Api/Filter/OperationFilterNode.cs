using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class OperationFilterNode : FilterNode
    {
        public OperationFilterNode(string path, FilterOperator op, string value, string? schema = null)
        {
            Path = path;
            Op = op;
            Value = value;
            Schema = schema;
        }

        public string Path { get; }

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

            throw new NotSupportedException($"Filter for {this.Path} not supported!");
        }
    }
}
