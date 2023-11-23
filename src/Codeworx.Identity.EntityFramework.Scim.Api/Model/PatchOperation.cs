using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class PatchOperation
    {
        public string[] Schemas { get; set; } = new string[] { SchemaConstants.PatchOperation };

        public List<PatchOperationDetail> Operations { get; set; }
    }
}
