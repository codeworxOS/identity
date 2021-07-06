using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public interface IExtendableObject
    {
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
