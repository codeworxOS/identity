﻿using System.Collections.Generic;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class UserResource : IUserResource, ISchemaResource, IResourceType
    {
        public string? UserName { get; set; }

        public bool Active { get; set; }

        public string? DisplayName { get; set; }

        public string? Password { get; set; }

        public string? NickName { get; set; }

        public string? ProfileUrl { get; set; }

        public string? Title { get; set; }

        public string? UserType { get; set; }

        public string? PreferredLanguage { get; set; }

        public string? ExternalId { get; set; }

        public string? Locale { get; set; }

        public NameResource? Name { get; set; }

        public string? Timezone { get; set; }

        public List<EmailResource>? Emails { get; set; }

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.User;

        string ISchemaResource.Schema => ScimConstants.Schemas.User;
    }
}
