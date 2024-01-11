using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public class RemoveProcessor
    {
        private readonly IFilterParser _parser;

        public RemoveProcessor(JsonObject json, IFilterParser parser)
        {
            Json = json;
            _parser = parser;
        }

        public JsonObject Json { get; }

        public Task ProcessAsync(PatchOperation operation)
        {
            if (operation.Value != null)
            {
                throw new ScimException(ScimType.InvalidValue);
            }

            if (operation.Path == null)
            {
                throw new ScimException(ScimType.NoTarget);
            }

            var parsedPath = _parser.Parse(operation.Path);

            if (parsedPath is ArrayFilterNode arrayNode)
            {
                var array = arrayNode.Path.Evaluate(Json) as JsonArray;

                if (array == null)
                {
                    throw new ScimException(ScimType.NoTarget);
                }

                foreach (var row in arrayNode.GetItems(Json, false).ToList())
                {
                    if (arrayNode.Member == null)
                    {
                        array.Remove(row);
                    }
                    else
                    {
                        row.AsObject().Remove(arrayNode.Member);
                    }
                }
            }
            else if (parsedPath is PathFilterNode pathNode)
            {
                var parent = pathNode.GetParent(Json, false);
                if (parent == null)
                {
                    throw new ScimException(ScimType.NoTarget);
                }

                parent.Remove(pathNode.Paths.Last());
            }
            else
            {
                throw new ScimException(ScimType.InvalidPath);
            }

            return Task.CompletedTask;
        }
    }
}
