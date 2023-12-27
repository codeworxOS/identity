using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    [JsonConverter(typeof(ResponseResourceConverter<UserResource>))]
    public class UserResponse : ScimResponse<UserResource>, IUserResource, ICommonResponseResource
    {
        public UserResponse(ScimResponseInfo info, UserResource user, params ISchemaResource[] extensions)
            : base(info, user, extensions)
        {
        }

        public string[] Schemas => Common.Schemas;

        public string? ExternalId => Resource.ExternalId;

        public string? UserName => Resource.UserName;

        public bool Active => Resource.Active;

        public string? DisplayName => Resource.DisplayName;

        public string? Password => Resource.Password;

        public string? NickName => Resource.NickName;

        public string? ProfileUrl => Resource.ProfileUrl;

        public string? Title => Resource.Title;

        public string? UserType => Resource.UserType;

        public string? PreferredLanguage => Resource.PreferredLanguage;

        public string? Locale => Resource.Locale;

        public NameResource? Name => Resource.Name;

        public string? Timezone => Resource.Timezone;

        public List<EmailResource>? Emails => Resource.Emails;

        public ScimMetadata Meta => Common.Meta;

        public string Id => Common.Id;
    }
}
