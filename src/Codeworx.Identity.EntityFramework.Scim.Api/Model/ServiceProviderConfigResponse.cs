using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class ServiceProviderConfigResponse
    {
        public ServiceProviderConfigResponse()
        {
            Schemas = new string[]
            {
                SchemaConstants.ServiceProviderConfig,
            };
            Patch = new PatchConfig { Supported = true };
            Bulk = new BulkConfig { Supported = false, MaxOperations = 1, MaxPayloadSize = 1 };
            Filter = new FilterConfig { Supported = true, MaxResults = 100 };
            ChangePassword = new ChangePasswordConfig { Supported = false };
            Sort = new SortConfig { Supported = false };
            Etag = new EtagConfig { Supported = false };
            AuthenticationSchemes = new List<AuthenticationSchemeConfig>();
        }

        public string[] Schemas { get; set; }

        public PatchConfig Patch { get; set; }

        public BulkConfig Bulk { get; set; }

        public FilterConfig Filter { get; set; }

        public ChangePasswordConfig ChangePassword { get; set; }

        public SortConfig Sort { get; set; }

        public EtagConfig Etag { get; set; }

        public ICollection<AuthenticationSchemeConfig> AuthenticationSchemes { get; set; }
    }
}
