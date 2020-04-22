using Microsoft.IdentityModel.JsonWebTokens;

namespace Codeworx.Identity
{
    public static class Constants
    {
        public const string AuthenticationExceptionMessage = "Username or password incorrect.";

        public const string ClaimSourceUrl = "http://schemas.codeworx.org/claims/source";
        public const string ClaimTargetUrl = "http://schemas.codeworx.org/claims/target";
        public const string DefaultAdminRoleId = "25E27405-3E81-4C50-8AD5-8C71DCD2191C";
        public const string DefaultAdminUserId = "DD772FD1-F823-46D0-A8C9-CC0C51C5C820";
        public const string DefaultAdminUserName = "admin";
        public const string DefaultAuthenticationCookieName = "identity";
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
        public const string ExternalOAuthProviderId = "d1e8741e03b5466aa7e3098787ef100d";
        public const string InvalidCredentialsError = "Username or password is not valid!";
        public const string JsonExtension = ".json";
        public const string MultiTenantUserId = "23EE9129-E14A-4FE4-9C16-D3473014C57F";
        public const string MultiTenantUserName = "multitenant";
        public const string ProductName = "CodeworxIdentity";
        public const string ReturnUrlParameter = "returnurl";
        public const string TenantNameProperty = "tenantName";
        public const string UserNameParameterName = "username";
        public const string WindowsAuthenticationSchema = "Windows";

        public class Token
        {
            public const string Jwt = "jwt";
        }

        public class Claims
        {
            public const string Id = "id";
            public const string Subject = JwtRegisteredClaimNames.Sub;
            public const string Name = "name";
            public const string Role = "role";
            public const string Tenant = "tenant";
            public const string CurrentTenant = "currenttenant";
        }
    }
}