using System;
using System.Collections.Generic;
using System.Linq;
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

        public override JsonNode? Evaluate(JsonObject json)
        {
            var parent = GetParent(json, false);

            if (parent != null && parent.TryGetPropertyValue(this.Paths.Last(), out var result))
            {
                return result;
            }

            return null;
        }

        public override void SetValue(JsonObject json, JsonNode value)
        {
            var parent = GetParent(json, true);

            var member = this.Paths.Last();

            if (parent!.TryGetPropertyValue(member, out var result))
            {
                parent.Remove(member);
            }

            parent.Add(member, value);
        }

        public JsonObject? GetParent(JsonObject json, bool createIfNotExists)
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

            for (int i = 0; i < Paths.Length - 1; i++)
            {
                if (!json.TryGetPropertyValue(Paths[i], out var nextNode))
                {
                    if (createIfNotExists)
                    {
                        nextNode = new JsonObject();
                        json.Add(Paths[i], nextNode);
                    }
                    else
                    {
                        return null;
                    }
                }

                json = nextNode!.AsObject();
            }

            return json;
        }

        public override string ToString()
        {
            var split = Scheme != null ? ":" : string.Empty;

            return $"{Scheme}{split}{string.Join(".", Paths)}";
        }

        protected override IEnumerable<FilterNode> GetChildren()
        {
            yield break;
        }
    }
}
