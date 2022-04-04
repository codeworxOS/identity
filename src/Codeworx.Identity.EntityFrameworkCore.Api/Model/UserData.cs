using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class UserData : IExtendableObject
    {
        public UserData()
        {
            this.AdditionalProperties = new Dictionary<string, object>();
            this.Tenants = new List<TenantInfoData>();
            this.Groups = new List<GroupInfoData>();
            this.Invitations = new List<InvitationData>();
        }

        public Guid Id { get; set; }

        [Required]
        public string Login { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public bool IsDisabled { get; set; }

        public bool ConfirmationPending { get; set; }

        public bool ForceChangePassword { get; set; }

        public DateTime Created { get; set; }

        public DateTime PasswordChanged { get; set; }

        public DateTime? LastFailedLoginAttempt { get; set; }

        public int FailedLoginCount { get; set; }

        public IList<TenantInfoData> Tenants { get; }

        public IList<GroupInfoData> Groups { get; }

        public IList<InvitationData> Invitations { get; }

        [JsonExtensionData]
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
