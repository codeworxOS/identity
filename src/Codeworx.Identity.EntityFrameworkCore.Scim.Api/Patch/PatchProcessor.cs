using System.Text.Json;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public class PatchProcessor : IPatchProcessor
    {
        private readonly ISerializationSetup _serialization;
        private readonly IFilterParser _parser;

        public PatchProcessor(ISerializationSetup serialization, IFilterParser parser)
        {
            _serialization = serialization;
            _parser = parser;
        }

        public async Task<TOutput> ProcessAsync<TInput, TOutput>(TInput resource, PatchResource patch)
        {
            var options = _serialization.GetOptionsForSerialize();

            var json = JsonSerializer.SerializeToNode<TInput>(resource, options)!.AsObject();

            var remove = new RemoveProcessor(json, _parser);
            var add = new AddProcessor(json, _parser);
            var replace = new ReplaceProcessor(json, _parser);

            foreach (var operation in patch.Operations)
            {
                switch (operation.Op)
                {
                    case PatchOp.Add:
                        await add.ProcessAsync(operation);
                        break;
                    case PatchOp.Remove:
                        await remove.ProcessAsync(operation);
                        break;
                    case PatchOp.Replace:
                        await replace.ProcessAsync(operation);
                        break;
                    default:
                        throw new ScimException(ScimType.InvalidSyntax);
                }
            }

            var deserializeOptiions = _serialization.GetOptionsForDeserialize();

            var result = JsonSerializer.Deserialize<TOutput>(json, deserializeOptiions)!;

            return result;
        }
    }
}
