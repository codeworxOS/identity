namespace Codeworx.Identity
{
    public static class Constants
    {
        public const string AuthenticationExceptionMessage = "Username or password incorrect.";

        public const string BasicHeader = "Basic";
        public const string DefaultAuthenticationCookieName = "identity";
        public const string DefaultMfaAuthenticationCookieName = DefaultAuthenticationCookieName + ".mfa";
        public const string DefaultAuthenticationScheme = "Codeworx.Identity";
        public const string DefaultMfaAuthenticationScheme = DefaultAuthenticationScheme + ".Mfa";
        public const string DefaultFavicon = "data:;base64,iVBORw0KGgo=";
        public const string DefaultPasswordDescriptionDe = "Das Passwort muss mindestens 8 Zeichen lang sein und mindestens eine Ziffer, einen Groß- und einen Kleinbuchstaben enthalten.";
        public const string DefaultPasswordDescriptionEn = "The password must be at least 8 characters long and contain at least one digit, one uppercase and one lowercase character.";
        public const string DefaultPasswordRegex = "(?=(.*[0-9]))((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.{8,}$";
        public const string DefaultLoginDescriptionDe = "Der Benutzername muss mindestens 4, maximal 20 Zeichen lang sein und kann Groß- und Kleinbuchstaben sowie . und _ enthalten.";
        public const string DefaultLoginDescriptionEn = "The login must be at least 4 but no more than 20 characters long and may contain . and _ as well as uppercase and one lowercase characters.";
        public const string DefaultLoginRegex = "^(?=[a-zA-Z0-9._]{4,20}$)(?!.*[_.]{2})[^_.].*[^_.]$";
        public const string EmailRegex = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";
        public const string InvitationParameter = "invitation";
        public const string JsonExtension = ".json";
        public const string Localhost = "localhost";
        public const string LoginProviderErrorParameter = "login_provider_error";
        public const string LoginProviderIdParameter = "login_provider_id";
        public const string NoNavParameter = "no_nav";
        public const string InvalidReturnUrlError = "invalid_return_url";

        public const string ReturnUrlParameter = "returnurl";
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
            public const string WebFonts = "/identity/webfonts";
        }

        public static class Claims
        {
            public const string AtHash = "at_hash";
            public const string Audience = "aud";
            public const string CurrentTenant = "current_tenant";
            public const string DefaultTenant = "default_tenant";
            public const string ExternalToken = "external_token";
            public const string ExternalTokenKey = "external_token_key";
            public const string ForceChangePassword = "force_change_password";
            public const string ForceMfaLogin = "force_mfa_login";
            public const string ConfirmationPending = "confirmation_pending";
            public const string Group = "group";
            public const string GroupNames = "group_names";
            public const string Id = "id";
            public const string Issuer = "iss";
            public const string Name = "name";
            public const string Subject = "sub";
            public const string Tenant = "tenant";
            public const string Upn = "upn";
            public const string Amr = "amr";
            public const string Trk = "trk";
            public const string Session = "session";
        }

        public static class Cache
        {
            public const string MailMfaPrefix = "mail_mfa_code";
        }

        public static class Forms
        {
            public const string ConfirmPassword = "confirm-password";
            public const string CurrentPassword = "current-password";
            public const string Password = "password";
            public const string ProviderId = "provider-id";
            public const string UserName = "username";
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
            public const string ActorTokenName = "actor_token";
            public const string ActorTokenTypeName = "actor_token_type";
            public const string ActorTokenTypeValidation = VsChars;
            public const string ActorTokenValidation = VsChars;
            public const string AdditionalParameterValidation = GrantTypeValidation;
            public const string AudienceName = "audience";
            public const string AudienceValidation = VsChars;
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
            public const string RequestedTokenTypeValidation = VsChars;
            public const string RequestedTokenTypeName = "requested_token_type";
            public const string ScopeName = "scope";
            public const string ScopeValidation = @"^(?<scope>" + NqChar + @"+)(\s{1}(?<scope>" + NqChar + @"+))*$";
            public const string StateName = "state";
            public const string StateValidation = VsChars;
            public const string SubjectTokenName = "subject_token";
            public const string SubjectTokenTypeName = "subject_token_type";
            public const string SubjectTokenTypeValidation = VsChars;
            public const string SubjectTokenValidation = VsChars;
            public const string TokenTypeName = "token_type";
            public const string TokenTypeValidation = GrantTypeValidation;
            public const string TokenName = "token";

            public const string UserNameValidation = UnicodeCharsNoCrlf;

            public class Error
            {
                public const string AccessDenied = "access_denied";
                public const string InvalidClient = "invalid_client";
                public const string InvalidGrant = "invalid_grant";
                public const string InvalidRequest = "invalid_request";
                public const string InvalidScope = "invalid_scope";
                public const string InvalidTarget = "invalid_target";
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
                public const string TokenExchange = "urn:ietf:params:oauth:grant-type:token-exchange";
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
                public const string MfaAuthenticationRequired = "mfa_authentication_required";
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

            public static class Amr
            {
                public const string Mfa = "mfa";
                public const string Otp = "otp";
                public const string Mail = "mail";
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

        public static class Processors
        {
            public const string Totp = "totp";

            public const string Mail = "mail";

            public const string Windows = "windows";

            public const string Forms = "forms";

            public const string OAuth = "oauth";
        }

        public static class Scopes
        {
            public const string Groups = "groups";
            public const string GroupNames = "group_names";
            public const string Tenant = "tenant";
            public const string ProfileEmail = "profile:email";

            public static class ExternalToken
            {
                public const string AccessToken = "external_token:access_token";
                public const string All = "external_token";
                public const string IdToken = "external_token:id_token";
            }
        }

        public static class Templates
        {
            public const string ConfirmAccountNotification = "confirm_account_notification";
            public const string NewInvitationNotification = "new_invitation_notification";
            public const string ForgotPasswordNotification = "forgot_password_notification";
            public const string MfaMailNotification = "mfa_mail_notification";
            public const string FormsInvitation = "formsinvitation";
            public const string FormsLogin = "formslogin";
            public const string FormsProfile = "formsprofile";
            public const string Redirect = "redirect";
            public const string RedirectProfile = "redirectprofile";
            public const string RegisterMfaMail = "register_mfa_mail";
            public const string LoginMfaMail = "login_mfa_mail";
            public const string MfaList = "mfa_list";
            public const string ProgressSpinner = "progress";
        }

        public static class Token
        {
            public const string Jwt = "jwt";

            public const string Reference = "ref";
        }

        public class Icons
        {
            public const string Windows = "fa-activedirectory";
        }

        public class TokenExchange
        {
            public class TokenType
            {
                public const string AccessToken = "urn:ietf:params:oauth:token-type:access_token";
                public const string IdToken = "urn:ietf:params:oauth:token-type:id_token";
            }
        }
    }
}