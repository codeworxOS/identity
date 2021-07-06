using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class GroupData : IExtendableObject
    {
        public GroupData()
        {
            this.AdditionalProperties = new Dictionary<string, object>();
            this.MemberOf = new List<GroupInfoData>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<GroupInfoData> MemberOf { get; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]

        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
