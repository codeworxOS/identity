using Codeworx.Identity.EntityFrameworkCore.Scim.Api;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class EnterpriseUserResource : ISchemaResource
    {
        public string? EmployeeNumber { get; set; }

        public string? CostCenter { get; set; }

        public string? Organization { get; set; }

        public string? Division { get; set; }

        public string? Department { get; set; }

        public MangerResource? Manager { get; set; }

        public string Schema => ScimConstants.Schemas.EnterpriseUser;
    }
}
