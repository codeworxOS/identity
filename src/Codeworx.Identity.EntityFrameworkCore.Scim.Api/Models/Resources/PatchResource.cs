using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class PatchResource : ISchemaResource, IPatchResource
    {
        public List<PatchOperation> Operations { get; set; } = new List<PatchOperation>();

        public string Schema => ScimConstants.Schemas.PatchOperation;
    }
}
