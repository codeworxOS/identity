namespace Codeworx.Identity.OAuth
{
    public static class Constants
    {
        public const string AccessTokenName = "access_token";
        public const string AccessTokenValidation = VsChars;
        public const string IdTokenName = "id_token";
        public const string IdTokenValidation = VsChars;
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
        public const string PasswordValidation = UnicodeCharsNoCrlf;
        public const string RedirectUriName = "redirect_uri";
        public const string RedirectUriValidation = UriReference;
        public const string RefreshTokenName = "refresh_token";
        public const string RefreshTokenValidation = VsChars;
        public const string ResponseTypeName = "response_type";
        public const string ResponseTypeValidation = @"^(?<response>[a-zA-Z0-9_]+){1}(\s{1}(?<response>[a-zA-Z0-9_]+))*$";
        public const string ScopeName = "scope";
        public const string ScopeValidation = @"^(?<scope>" + NqChar + @"+)(\s{1}(?<scope>" + NqChar + @"+))*$";
        public const string StateName = "state";
        public const string StateValidation = VsChars;
        public const string NonceName = "nonce";
        public const string NonceValidation = VsChars;
        public const string ResponseModeName = "response_mode";
        public const string ResponseModeValidation = VsChars;

        public const string TokenTypeName = "token_type";
        public const string TokenTypeValidation = GrantTypeValidation;

        public const string UserNameValidation = UnicodeCharsNoCrlf;

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

            public const string Password = "password";
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

        public class ReservedClaims
        {
            public const string UserId = "sub";
        }
    }
}