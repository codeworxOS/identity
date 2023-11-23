using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class UserResponse : IResource
    {
        public UserResponse()
        {
            Schemas = new string[]
            {
                SchemaConstants.User,
            };
            AdditionalProperties = new Dictionary<string, object>();
        }

        public string[] Schemas { get; set; }

        public Guid Id { get; set; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}