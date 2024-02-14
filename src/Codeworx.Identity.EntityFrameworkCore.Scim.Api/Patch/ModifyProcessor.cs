using System.Linq;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public class ModifyProcessor
    {
        public ModifyProcessor(IFilterParser parser)
        {
            Parser = parser;
        }

        protected IFilterParser Parser { get; }

        protected void MergeProperties(JsonObject node, JsonObject target)
        {
            foreach (var item in node.ToList())
            {
                node.Remove(item.Key);

                var parsed = Parser.Parse(item.Key);

                if (parsed is PathFilterNode path)
                {
                    SetPropertyValue(path, target, item.Value!);
                }
            }
        }

        protected void SetPropertyValue(PathFilterNode path, JsonObject target, JsonNode value)
        {
            var parent = path.GetParent(target, true);

            if (parent == null)
            {
                throw new ScimException(ScimType.InvalidPath);
            }

            parent.Remove(path.Paths.Last());
            parent.Add(path.Paths.Last(), value);
        }
    }
}
