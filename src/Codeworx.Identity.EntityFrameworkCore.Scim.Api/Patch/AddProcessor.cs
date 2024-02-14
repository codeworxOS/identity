using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public class AddProcessor : ModifyProcessor
    {
        public AddProcessor(JsonObject json, IFilterParser parser)
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
                    var member = pathNode.Paths.Last();
                    var parent = pathNode.GetParent(Json, true)!;

                    if (parent.TryGetPropertyValue(member, out var value))
                    {
                        if (value is JsonArray subArray)
                        {
                            if (node is JsonObject nodeObject)
                            {
                                subArray.Add(nodeObject);
                            }
                            else if (node is JsonArray nodeArray)
                            {
                                foreach (var row in nodeArray.ToList())
                                {
                                    nodeArray.Remove(row);
                                    subArray.Add(row);
                                }
                            }
                            else
                            {
                                throw new ScimException(ScimType.InvalidValue);
                            }
                        }
                        else if (value is JsonObject subObject)
                        {
                            MergeProperties(node.AsObject(), subObject);
                        }
                        else
                        {
                            SetPropertyValue(pathNode, Json, node);
                        }
                    }
                    else
                    {
                        SetPropertyValue(pathNode, Json, node);
                    }
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
