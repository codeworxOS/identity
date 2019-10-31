namespace Codeworx.Identity
{
    public static class Constants
    {
        public const string AuthenticationExceptionMessage = "Username or password incorrect.";

        public const string ClaimSourceUrl = "http://schemas.codeworx.org/claims/source";
        public const string ClaimTargetUrl = "http://schemas.codeworx.org/claims/target";
        public const string ConfigurationExceptionMessage = "Identity Configuration Incomplete.";
        public const string ConfigurationIdentityProviderMissingExceptionMessage = "No IIdentityProvider configured";
        public const string ConfigurationViewMissingExceptionMessage = "No IViewTemplate configured.";
        public const string CurrentTenantClaimType = "currenttenant";
        public const string DefaultAdminUserId = "DD772FD1-F823-46D0-A8C9-CC0C51C5C820";
        public const string DefaultAdminUserName = "admin";
        public const string DefaultAdminRoleId = "25E27405-3E81-4C50-8AD5-8C71DCD2191C";
        public const string DefaultAuthenticationScheme = "Codeworx.Identity";
        public const string DefaultCodeFlowClientId = "EADB8036-4AA6-4468-9349-43FF541EBF5E";
        public const string DefaultScopeKey = "all";
        public const string DefaultSecondTenantId = "BC7B302B-1120-4C66-BBE4-CD25D50854CE";
        public const string DefaultSecondTenantName = "Sendond Tenant";
        public const string DefaultTenantId = "F124DF47-A99E-48EE-88B4-97901764E484";
        public const string DefaultTenantName = "Default";
        public const string DefaultTokenFlowClientId = "B45ABA81-AAC1-403F-93DD-1CE42F745ED2";
        public const string ExternalWindowsProviderId = "d740e319bbc44ab0b815136cb1f96d2e";
        public const string ExternalWindowsProviderName = "Windows";
        public const string IdClaimType = "id";
        public const string JsonExtension = ".json";
        public const string LoginClaimType = "name";
        public const string DefaultMissingTenantAuthenticationScheme = "Identity.Missing.Tenant";
        public const string DefaultMissingTenantCookieName = "identity.missingtenant";
        public const string DefaultAuthenticationCookieName = "identity";
        public const string MultiTenantUserId = "23EE9129-E14A-4FE4-9C16-D3473014C57F";
        public const string MultiTenantUserName = "multitenant";
        public const string ProductName = "CodeworxIdentity";
        public const string ReturnUrlParameter = "returnurl";
        public const string RoleClaimType = "role";
        public const string TenantClaimType = "tenant";
        public const string TenantNameProperty = "tenantName";
        public const string UserNameParameterName = "username";
        public const string WindowsAuthenticationSchema = "Windows";

        public class Token
        {
            public const string Jwt = "jwt";
        }
    }
}