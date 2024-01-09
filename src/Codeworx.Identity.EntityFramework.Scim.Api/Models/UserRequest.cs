using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    [JsonConverter(typeof(RequestResourceConverter<UserResource>))]
    public class UserRequest : ScimRequest<UserResource>, IUserResource, ICommonRequestResource
    {
        public UserRequest(CommonRequestResource common, UserResource user, params ISchemaResource[] extensions)
            : base(common, user, extensions)
        {
        }

        public string[] Schemas { get => Common.Schemas; }

        public string? ExternalId { get => Resource.ExternalId; }

        public string? UserName { get => Resource.UserName; }

        public bool? Active { get => Resource.Active; }

        public string? DisplayName { get => Resource.DisplayName; }

        public string? Password { get => Resource.Password; }

        public string? NickName { get => Resource.NickName; }

        public string? ProfileUrl { get => Resource.ProfileUrl; }

        public string? Title { get => Resource.Title; }

        public string? UserType { get => Resource.UserType; }

        public string? PreferredLanguage { get => Resource.PreferredLanguage; }

        public string? Locale { get => Resource.Locale; }

        public NameResource? Name { get => Resource.Name; }

        public string? Timezone { get => Resource.Timezone; }

        public List<EmailResource>? Emails { get => Resource.Emails; }
    }
}
