using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface IUserResource
    {
        string? UserName { get; }

        bool? Active { get; }

        string? DisplayName { get; }

        string? Password { get; }

        string? NickName { get; }

        string? ProfileUrl { get; }

        string? Title { get; }

        string? UserType { get; }

        string? PreferredLanguage { get; }

        string? Locale { get; }

        NameResource? Name { get; }

        string? Timezone { get; }

        public string? ExternalId { get; }

        List<EmailResource?>? Emails { get; }
    }
}
