using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class TenantInsertData : IExtendableObject
    {
        public TenantInsertData()
        {
            this.AdditionalProperties = new Dictionary<string, object>();
        }

        [Required]
        public string Name { get; set; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}