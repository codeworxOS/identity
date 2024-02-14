using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public class ReplaceProcessor : ModifyProcessor
    {
        public ReplaceProcessor(JsonObject json, IFilterParser parser)
            : base(parser)
        {
            Json = json;
        }

        public JsonObject Json { get; }

        public Task ProcessAsync(PatchOperation operation)
        {
            if (operation.Value == null)
            {
                throw new ScimException(ScimType.InvalidValue);
            }

            JsonNode? node = null;

            if (operation.Value is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    node = JsonObject.Create(element);
                }
                else if (element.ValueKind == JsonValueKind.Array)
                {
                    node = JsonArray.Create(element);
                }
                else
                {
                    node = JsonValue.Create(element);
                }
            }

            if (node == null)
            {
                throw new ScimException(ScimType.InvalidValue);
            }

            if (operation.Path != null)
            {
                var parsed = Parser.Parse(operation.Path);

                if (parsed is ArrayFilterNode arrayNode)
                {
                    var items = arrayNode.GetItems(Json, true).ToList();

                    foreach (var item in items)
                    {
                        if (arrayNode.Member != null)
                        {
                            var memberNode = (PathFilterNode)Parser.Parse(arrayNode.Member);
                            SetPropertyValue(memberNode, item.AsObject(), node);
                        }
                        else
                        {
                            MergeProperties(node.AsObject(), item.AsObject());
                        }
                    }
                }
                else if (parsed is PathFilterNode pathNode)
                {
                    pathNode.SetValue(Json, node);
                }
                else
                {
                    throw new ScimException(ScimType.InvalidPath);
                }
            }
            else
            {
                var valueObject = node as JsonObject;

                if (valueObject == null)
                {
                    throw new ScimException(ScimType.InvalidValue);
                }

                MergeProperties(valueObject, Json);
            }

            return Task.CompletedTask;
        }
    }
}
