using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class UserListData : IExtendableObject
    {
        public UserListData()
        {
            this.AdditionalProperties = new Dictionary<string, object>();
        }

        public Guid Id { get; set; }

        public string Login { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public bool IsDisabled { get; set; }

        public bool ConfirmationPending { get; set; }

        public bool HasOpenInvitation { get; set; }

        public DateTime Created { get; set; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
