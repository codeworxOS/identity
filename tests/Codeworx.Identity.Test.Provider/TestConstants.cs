namespace Codeworx.Identity.Test.Provider
{
    public class TestConstants
    {
        public static class Groups
        {
            public const string DefaultAdminGroupId = "25E27405-3E81-4C50-8AD5-8C71DCD2191C";
        }

        public static class LoginProviders
        {
            public static class ExternalOAuthProvider
            {
                public const string Id = "d1e8741e03b5466aa7e3098787ef100d";
                public const string Name = "Basic OAuth";
            }

            public static class ExternalWindowsProvider
            {
                public const string Id = "d740e319bbc44ab0b815136cb1f96d2e";
                public const string Name = "Windows";
            }

            public static class FormsLoginProvider
            {
                public const string Id = "55efbccf5f4a4ec2ba412ed4f56dfa92";
                public const string Name = "Form";
            }

            public static class TotpProvider
            {
                public const string Id = "1f7ac4c2-1fe5-4931-9f25-4bee1e1af992";
                public const string Name = "Totp";
            }
        }

        public static class Clients
        {
            public const string DefaultCodeFlowClientId = "eadb80364aa64468934943ff541ebf5e";
            public const string DefaultCodeFlowPublicClientId = "809b3854c35449b990dc83f80ac5f4c2";
            public const string DefaultServiceAccountClientId = "adad0b0fe1364726b49ffa45253e0dbd";
            public const string DefaultTokenFlowClientId = "b45aba81aac1403f93dd1ce42f745ed2";
            public const string LimitedScope1ClientId = "B25ECAB6-76A4-498A-AA50-B8699E0BDDA6";
        }

        public static class Users
        {
            public static class DefaultAdmin
            {
                public const string UserId = "DD772FD1-F823-46D0-A8C9-CC0C51C5C820";
                public const string UserName = "admin";
                public const string Password = "admin";
            }

            public static class DefaultServiceAccount
            {
                public const string UserId = "F3C7DD5E-2678-41F8-8018-807C711CF733";
                public const string UserName = "service.account";
            }

            public static class DefaultEmail
            {
                public const string UserId = "66035864-7251-4287-96ED-D6EB77B7555B";
                public const string UserName = "test@provider.com";
                public const string Password = "emailuser";
            }

            public static class ForceChangePassword
            {
                public const string UserId = "1CDDC1E8-ABF5-4AEE-AB84-07D6AA9DD932";
                public const string UserName = "changepassword";
                public const string Password = "changepassword";
            }

            public static class MultiTenant
            {
                public const string UserId = "23EE9129-E14A-4FE4-9C16-D3473014C57F";
                public const string UserName = "multitenant";
                public const string Password = "multitenant";
            }

            public static class NotExisting
            {
                public const string UserName = "notExistingUser";
            }

            public static class NoPassword
            {
                public const string UserId = "65AB0982-F4CE-4ADA-B94E-C77CC3A5F075";
                public const string UserName = "nopassword";
            }
        }

        public static class Tenants
        {
            public static class DefaultTenant
            {
                public const string Id = "F124DF47-A99E-48EE-88B4-97901764E484";
                public const string Name = "Default";
            }

            public static class DefaultSecondTenant
            {
                public const string Id = "BC7B302B-1120-4C66-BBE4-CD25D50854CE";
                public const string Name = "Sendond Tenant";
            }
        }
    }
}
