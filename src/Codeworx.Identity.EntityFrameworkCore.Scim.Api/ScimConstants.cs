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

            public const string Error = "urn:ietf:params:scim:api:messages:2.0:Error";
        }

        public class ResourceTypes
        {
            public const string User = "User";
            public const string Group = "Group";
            public const string Schema = "Schema";
            public const string ResourceType = "ResourceType";
            public const string EnterpriseUser = "EnterpriseUser";
        }

        public class Policies
        {
            public const string ScimInterop = "identity_api_scim_policy";
        }

        public class Error
        {
            public const string InvalidFilter = "invalidFilter";
            public const string TooMany = "tooMany";
            public const string Uniqueness = "uniqueness";
            public const string Mutability = "mutability";
            public const string InvalidSyntax = "invalidSyntax";
            public const string InvalidPath = "invalidPath";
            public const string NoTarget = "noTarget";
            public const string InvalidValue = "invalidValue";
            public const string InvalidVers = "invalidVers";
            public const string Sensitive = "sensitive";
        }
    }
}
