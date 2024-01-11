using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class PathFilterNode : ValueFilterNode
    {
        public PathFilterNode(string[] paths, string? scheme = null)
        {
            Paths = paths;
            Scheme = scheme;
            Members = string.Join(".", paths);
        }

        public string Members { get; }

        public string[] Paths { get; }

        public string? Scheme { get; }

        public override LambdaExpression Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            throw new NotImplementedException();
        }

        public override object? Evaluate(JsonObject json)
        {
            if (Scheme != null)
            {
                if (!json.TryGetPropertyValue(Scheme, out var schemeValue))
                {
                    schemeValue = new JsonObject();
                    json.Add(Scheme, schemeValue);
                }

                json = schemeValue!.AsObject();
            }

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
                    return json[Paths[i]];
                }
            }

            return null;
        }

        public override string ToString()
        {
            var split = Scheme != null ? ":" : string.Empty;

            return $"{Scheme}{split}{string.Join(".", Paths)}";
        }
    }
}
