using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class UserInsertData : IExtendableObject
    {
        public UserInsertData()
        {
            this.AdditionalProperties = new Dictionary<string, object>();
        }

        [Required]
        public string Login { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public bool IsDisabled { get; set; }

        public bool ForceChangePassword { get; set; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
