namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    public static class ScimConstants
    {
        public class Schemas
        {
            public const string List = "urn:ietf:params:scim:api:messages:2.0:ListResponse";
            public const string User = "urn:ietf:params:scim:schemas:core:2.0:User";
            public const string Group = "urn:ietf:params:scim:schemas:core:2.0:Group";

            public const string Schema = "urn:ietf:params:scim:schemas:core:2.0:Schema";
            public const string ServiceProviderConfig = "urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig";
            public const string ResourceType = "urn:ietf:params:scim:schemas:core:2.0:ResourceType";

            public const string PatchOperation = "urn:ietf:params:scim:api:messages:2.0:PatchOp";

            public const string EnterpriseUser = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User";
        }

        public class ResourceTypes
        {
            public const string User = "User";
            public const string Group = "Group";
            public const string Schema = "Schema";
            public const string ResourceType = "ResourceType";
        }
    }
}
