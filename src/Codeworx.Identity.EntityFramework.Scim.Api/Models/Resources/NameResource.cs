namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class NameResource
    {
        public string? Formatted { get; set; }

        public string? FamilyName { get; set; }

        public string? GivenName { get; set; }

        public string? MiddleName { get; set; }

        public string? HonorificPrefix { get; set; }

        public string? HonorificSuffix { get; set; }
    }
}