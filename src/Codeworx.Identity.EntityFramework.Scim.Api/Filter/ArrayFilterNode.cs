using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class ArrayFilterNode : FilterNode
    {
        public ArrayFilterNode(string[] paths, FilterNode filter, string? member = null, string? schema = null)
        {
            Paths = paths;
            Filter = filter;
            Member = member;
            Schema = schema;
        }

        public string[] Paths { get; }

        public FilterNode Filter { get; }

        public string? Member { get; }

        public string? Schema { get; }

        public override Expression<Func<ScimEntity<TEntity>, bool>> Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<JsonNode> GetItems(JsonObject json)
        {
            JsonArray? array = null;

            for (int i = 0; i < Paths.Length; i++)
            {
                if (i < Paths.Length - 1)
                {
                    var next = json[Paths[i]]?.AsObject();

                    if (next == null)
                    {
                        throw new NotSupportedException("Invalid Path!");
                    }

                    json = next;
                }
                else
                {
                    array = json[Paths[i]]?.AsArray();
                }
            }

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

        public override bool Evaluate(JsonObject json)
        {
            if (Member != null)
            {
                throw new NotSupportedException("Invalid path!");
            }

            foreach (var item in GetItems(json))
            {
                return true;
            }

            return false;
        }
    }
}
