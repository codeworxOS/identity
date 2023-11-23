using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class GroupResponse : IResource
    {
        public GroupResponse()
        {
            Schemas = new string[]
            {
                SchemaConstants.Group,
            };
            AdditionalProperties = new Dictionary<string, object>();
        }

        public string[] Schemas { get; set; }

        public Guid Id { get; set; }

        public ICollection<GroupMember> Members { get; set; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}