using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public interface IResource
    {
        Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
