using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface IPatchResource
    {
        List<PatchOperation> Operations { get; }
    }
}