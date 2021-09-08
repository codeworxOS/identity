namespace Codeworx.Identity
{
    public static class Constants
    {
        public const string AuthenticationExceptionMessage = "Username or password incorrect.";

        public const string BasicHeader = "Basic";
        public const string ClaimSourceUrl = "http://schemas.codeworx.org/claims/source";
        public const string ClaimTargetUrl = "http://schemas.codeworx.org/claims/target";
        public const string DefaultAdminGroupId = "25E27405-3E81-4C50-8AD5-8C71DCD2191C";
        public const string DefaultAdminUserId = "DD772FD1-F823-46D0-A8C9-CC0C51C5C820";
        public const string DefaultAdminUserName = "admin";
        public const string DefaultAuthenticationCookieName = "identity";
        public const string DefaultAuthenticationScheme = "Codeworx.Identity";
        public const string DefaultCodeFlowClientId = "EADB8036-4AA6-4468-9349-43FF541EBF5E";
        public const string DefaultCodeFlowPublicClientId = "809B3854-C354-49B9-90DC-83F80AC5F4C2";
        public const string DefaultPasswordDescription = "The password must be at least 8 characters long and contain at least one digit, one uppercase and one lowercase character.";
        public const string DefaultPasswordRegex = "(?=(.*[0-9]))((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.{8,}$";
        public const string DefaultScopeKey = "all";
        public const string DefaultSecondTenantId = "BC7B302B-1120-4C66-BBE4-CD25D50854CE";
        public const string DefaultSecondTenantName = "Sendond Tenant";
        public const string DefaultServiceAccountClientId = "ADAD0B0F-E136-4726-B49F-FA45253E0DBD";
        public const string DefaultServiceAccountId = "F3C7DD5E-2678-41F8-8018-807C711CF733";
        public const string DefaultServiceAccountName = "service.account";
        public const string DefaultTenantId = "F124DF47-A99E-48EE-88B4-97901764E484";
        public const string DefaultTenantName = "Default";
        public const string DefaultTokenFlowClientId = "B45ABA81-AAC1-403F-93DD-1CE42F745ED2";
        public const string ExternalAccountAlreadyLinkedError = "The external account is already linked to a different user.";
        public const string ExternalAccountNotLinked = "Your Account is not yet linked.";
        public const string ExternalOAuthProviderId = "d1e8741e03b5466aa7e3098787ef100d";
        public const string ExternalWindowsProviderId = "d740e319bbc44ab0b815136cb1f96d2e";
        public const string ExternalWindowsProviderName = "Windows";
        public const string FormsLoginProviderId = "55efbccf5f4a4ec2ba412ed4f56dfa92";
        public const string FormsLoginProviderName = "Form";
        public const string GenericLoginError = "There was an unexpected login error!";
        public const string InvalidCredentialsError = "Username or password is not valid!";
        public const string InvitationNotSupported = "Invitation feature is not supported!";
        public const string InvitationParameter = "invitation";
        public const string JsonExtension = ".json";
        public const string Localhost = "localhost";
        public const string LoginProviderErrorParameter = "login_provider_error";
        public const string LoginProviderIdParameter = "login_provider_id";
        public const string MultiTenantUserId = "23EE9129-E14A-4FE4-9C16-D3473014C57F";
        public const string MultiTenantUserName = "multitenant";
        public const string ForcePasswordUserId = "1CDDC1E8-ABF5-4AEE-AB84-07D6AA9DD932";
        public const string ForcePasswordUserName = "changepassword";

        public const string ProductName = "CodeworxIdentity";
        public const string ReturnUrlParameter = "returnurl";
        public const string TenantNameProperty = "tenantName";
        public const string UnknownLoginProviderError = "Invalid provider!";
        public const string UserNameParameterName = "username";
        public const string WindowsAuthenticationSchema = "Windows";
        private const string NameChar = @"[a-zA-Z0-9_]";
        private const string NqChar = @"[\u0021\u0023-\u005b\u005d-\u007e]";

        private const string NqsChar = @"[\u0020-\u0021\u0023-\u005b\u005d-\u007e]";

        private const string NqsChars = @"^" + NqsChar + @"+$";

        private const string UnicodeCharsNoCrlf = @"^[\u0009\u0020-\u007e\u0080-\ud7ff\ue000-\ufffd\u10000-\u10ffff]+$";

        private const string Uri = @"(?:[A-Za-z][A-Za-z0-9+\-.]*:(?://(?:(?:[A-Za-z0-9\-._~!$&'()*+,;=:]|%[0-9A-Fa-f]{2})*@)?(?:\[(?:(?:(?:(?:[0-9A-Fa-f]{1,4}:){6}|::(?:[0-9A-Fa-f]{1,4}:){5}|(?:[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){4}|(?:(?:[0-9A-Fa-f]{1,4}:){0,1}[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){3}|(?:(?:[0-9A-Fa-f]{1,4}:){0,2}[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){2}|(?:(?:[0-9A-Fa-f]{1,4}:){0,3}[0-9A-Fa-f]{1,4})?::[0-9A-Fa-f]{1,4}:|(?:(?:[0-9A-Fa-f]{1,4}:){0,4}[0-9A-Fa-f]{1,4})?::)(?:[0-9A-Fa-f]{1,4}:[0-9A-Fa-f]{1,4}|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))|(?:(?:[0-9A-Fa-f]{1,4}:){0,5}[0-9A-Fa-f]{1,4})?::[0-9A-Fa-f]{1,4}|(?:(?:[0-9A-Fa-f]{1,4}:){0,6}[0-9A-Fa-f]{1,4})?::)|[Vv][0-9A-Fa-f]+\.[A-Za-z0-9\-._~!$&'()*+,;=:]+)\]|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|(?:[A-Za-z0-9\-._~!$&'()*+,;=]|%[0-9A-Fa-f]{2})*)(?::[0-9]*)?(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*|/(?:(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})+(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*)?|(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})+(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*|)(?:\?(?:[A-Za-z0-9\-._~!$&'()*+,;=:@/?]|%[0-9A-Fa-f]{2})*)?(?:\#(?:[A-Za-z0-9\-._~!$&'()*+,;=:@/?]|%[0-9A-Fa-f]{2})*)?|(?://(?:(?:[A-Za-z0-9\-._~!$&'()*+,;=:]|%[0-9A-Fa-f]{2})*@)?(?:\[(?:(?:(?:(?:[0-9A-Fa-f]{1,4}:){6}|::(?:[0-9A-Fa-f]{1,4}:){5}|(?:[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){4}|(?:(?:[0-9A-Fa-f]{1,4}:){0,1}[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){3}|(?:(?:[0-9A-Fa-f]{1,4}:){0,2}[0-9A-Fa-f]{1,4})?::(?:[0-9A-Fa-f]{1,4}:){2}|(?:(?:[0-9A-Fa-f]{1,4}:){0,3}[0-9A-Fa-f]{1,4})?::[0-9A-Fa-f]{1,4}:|(?:(?:[0-9A-Fa-f]{1,4}:){0,4}[0-9A-Fa-f]{1,4})?::)(?:[0-9A-Fa-f]{1,4}:[0-9A-Fa-f]{1,4}|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))|(?:(?:[0-9A-Fa-f]{1,4}:){0,5}[0-9A-Fa-f]{1,4})?::[0-9A-Fa-f]{1,4}|(?:(?:[0-9A-Fa-f]{1,4}:){0,6}[0-9A-Fa-f]{1,4})?::)|[Vv][0-9A-Fa-f]+\.[A-Za-z0-9\-._~!$&'()*+,;=:]+)\]|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|(?:[A-Za-z0-9\-._~!$&'()*+,;=]|%[0-9A-Fa-f]{2})*)(?::[0-9]*)?(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*|/(?:(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})+(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*)?|(?:[A-Za-z0-9\-._~!$&'()*+,;=@]|%[0-9A-Fa-f]{2})+(?:/(?:[A-Za-z0-9\-._~!$&'()*+,;=:@]|%[0-9A-Fa-f]{2})*)*|)(?:\?(?:[A-Za-z0-9\-._~!$&'()*+,;=:@/?]|%[0-9A-Fa-f]{2})*)?(?:\#(?:[A-Za-z0-9\-._~!$&'()*+,;=:@/?]|%[0-9A-Fa-f]{2})*)?)";

        // RFC3986 uri-reference regex pattern
        private const string UriReference = @"^" + Uri + "$";

        private const string VsChar = @"[\u0020-\u007e]";
        private const string VsChars = @"^" + VsChar + @"+$";

        public static class Assets
        {
            public const string Css = "/identity/css";
            public const string Images = "/identity/images";
            public const string Js = "/identity/js";
        }

        public static class Claims
        {
            public const string Audience = "aud";
            public const string CurrentTenant = "current_tenant";
            public const string DefaultTenant = "default_tenant";
            public const string ExternalToken = "external_token";
            public const string ExternalTokenKey = "external_token_key";
            public const string Group = "group";
            public const string Id = "id";
            public const string Issuer = "iss";
            public const string Name = "name";
            public const string Subject = "sub";
            public const string Tenant = "tenant";
            public const string Upn = "upn";
            public const string AtHash = "at_hash";
        }

        public static class Forms
        {
            public const string ConfirmPassword = "confirm-password";
            public const string CurrentPassword = "current-password";
            public const string Password = "password";
            public const string ProviderId = "provider-id";
        }

        public static class KeyType
        {
            public const string EllipticCurve = "EC";
            public const string Oct = "oct";
            public const string Rsa = "RSA";
        }

        public static class KeyUse
        {
            public const string Encryption = "enc";
            public const string Signature = "sig";
        }

        public static class OAuth
        {
            public const string AccessTokenName = "access_token";
            public const string AccessTokenValidation = VsChars;
            public const string AdditionalParameterValidation = GrantTypeValidation;
            public const string ClientIdName = "client_id";
            public const string ClientIdValidation = VsChars;
            public const string ClientSecretName = "client_secret";
            public const string ClientSecretValidation = VsChars;
            public const string CodeName = "code";
            public const string CodeValidation = VsChars;
            public const string EndpointValidation = "^" + NameChar + @"+$";
            public const string ErrorDescriptionName = "error_description";
            public const string ErrorDescriptionValidation = NqsChars;
            public const string ErrorName = "error";
            public const string ErrorUriName = "error_uri";
            public const string ErrorUriValidation = UriReference;
            public const string ErrorValidation = NqsChars;
            public const string ExpiresInName = "expires_in";
            public const string ExpiresInValidation = @"^[0-9]+$";
            public const string GrantTypeName = "grant_type";
            public const string GrantTypeValidation = @"^((?<grant>" + NameChar + @"+)|(?<grant>" + Uri + @")){1}$";
            public const string NonceName = "nonce";
            public const string NonceValidation = VsChars;
            public const string PasswordValidation = UnicodeCharsNoCrlf;
            public const string PromptName = "prompt";
            public const string PromptValidation = @"^(?<prompt>" + NqChar + @"+)(\s{1}(?<prompt>" + NqChar + @"+))*$";
            public const string RedirectUriName = "redirect_uri";
            public const string RedirectUriValidation = UriReference;
            public const string RefreshTokenName = "refresh_token";
            public const string RefreshTokenValidation = VsChars;
            public const string RequestPathName = "request_path";
            public const string ResponseModeName = "response_mode";
            public const string ResponseModeValidation = VsChars;
            public const string ResponseTypeName = "response_type";
            public const string ResponseTypeValidation = @"^(?<response>[a-zA-Z0-9_]+){1}(\s{1}(?<response>[a-zA-Z0-9_]+))*$";
            public const string ScopeName = "scope";
            public const string ScopeValidation = @"^(?<scope>" + NqChar + @"+)(\s{1}(?<scope>" + NqChar + @"+))*$";
            public const string StateName = "state";
            public const string StateValidation = VsChars;
            public const string TokenTypeName = "token_type";
            public const string TokenTypeValidation = GrantTypeValidation;

            public const string UserNameValidation = UnicodeCharsNoCrlf;

            public class Error
            {
                public const string AccessDenied = "access_denied";
                public const string InvalidClient = "invalid_client";
                public const string InvalidGrant = "invalid_grant";
                public const string InvalidRequest = "invalid_request";
                public const string InvalidScope = "invalid_scope";
                public const string ServerError = "server_error";
                public const string TemporarilyUnavailable = "temporarily_unavailable";
                public const string UnauthorizedClient = "unauthorized_client";
                public const string UnsupportedGrantType = "unsupported_grant_type";
                public const string UnsupportedResponseType = "unsupported_response_type";
            }

            public class GrantType
            {
                public const string AuthorizationCode = "authorization_code";
                public const string ClientCredentials = "client_credentials";
                public const string Password = "password";
                public const string RefreshToken = "refresh_token";
            }

            public class Prompt
            {
                public const string Consent = "consent";
                public const string Login = "login";
                public const string None = "none";
                public const string SelectAccount = "select_account";
            }

            public class ReservedClaims
            {
                public const string UserId = "sub";
            }

            public class ResponseMode
            {
                public const string Fragment = "fragment";

                public const string Query = "query";
            }

            public class ResponseType
            {
                public const string Code = "code";

                public const string Token = "token";
            }

            public class TokenType
            {
                public const string Bearer = "BEARER";
            }
        }

        public static class OpenId
        {
            public const string IdTokenName = "id_token";
            public const string IdTokenValidation = VsChars;

            public static class Error
            {
                public const string AccountSelectionRequired = "account_selection_required";
                public const string ConsentRequired = "consent_required";
                public const string InteractionRequired = "interaction_required";
                public const string InvalidRequestObject = "invalid_request_object";
                public const string InvalidRequestUri = "invalid_request_uri";
                public const string LoginRequired = "login_required";
                public const string RegistrationNotSupported = "registration_not_supported";
                public const string RequestNotSupported = "request_not_supported";
                public const string RequestUriNotSupported = "request_uri_not_supported";
            }

            public static class ResponseMode
            {
                public const string FormPost = "form_post";
            }

            public static class ResponseType
            {
                public const string IdToken = "id_token";
            }

            public static class Scopes
            {
                public const string OfflineAccess = "offline_access";
                public const string OpenId = "openid";
                public const string Profile = "profile";
            }
        }

        public static class Scopes
        {
            public const string Groups = "groups";
            public const string Tenant = "tenant";

            public static class ExternalToken
            {
                public const string AccessToken = "external_token:access_token";
                public const string All = "external_token";
                public const string IdToken = "external_token:id_token";
            }
        }

        public static class Templates
        {
            public const string FormsInvitation = "formsinvitation";
            public const string FormsLogin = "formslogin";
            public const string FormsProfile = "formsprofile";
            public const string Redirect = "redirect";
            public const string RedirectProfile = "redirectprofile";
        }

        public static class Token
        {
            public const string Jwt = "jwt";
        }

        public class Icons
        {
            public const string Windows = "icon-windows";
        }
    }
}