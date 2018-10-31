namespace Codeworx.Identity
{
    public static class Constants
    {
        public const string AuthenticationExceptionMessage = "Username or password incorrect.";

        public const string ClaimSourceUrl = "http://schemas.codeworx.org/claims/source";
        public const string ClaimTargetUrl = "http://schemas.codeworx.org/claims/target";
        public const string ConfigrationExceptionMessage = "Identity Configuration Incomplete.";
        public const string ConfigrationIdentityProviderMissingExceptionMessage = "No IIdentityProvider configured";
        public const string ConfigrationViewMissingExceptionMessage = "No IViewTemplate configured.";
        public const string CurrentTenantClaimType = "currenttenant";
        public const string DefaultAdminUserId = "DD772FD1-F823-46D0-A8C9-CC0C51C5C820";
        public const string DefaultAdminUserName = "admin";
        public const string DefaultAuthenticationScheme = "Codeworx.Identity";
        public const string DefaultTenantId = "F124DF47-A99E-48EE-88B4-97901764E484";
        public const string DefaultTenantName = "Default";
        public const string ExternalWindowsProviderId = "D740E319BBC44AB0B815136CB1F96D2E";
        public const string ExternalWindowsProviderName = "Windows";
        public const string IdClaimType = "sub";
        public const string JsonExtension = ".json";
        public const string NameClaimType = "name";
        public const string ProductName = "CodeworxIdentity";
        public const string ReturnUrlParameter = "returnurl";
        public const string RoleClaimType = "role";
        public const string TenantClaimType = "tenant";
        public const string TenantNameProperty = "tenantName";
        public const string UserNameParameterName = "username";
        public const string WindowsAuthenticationSchema = "Windows";
    }
}