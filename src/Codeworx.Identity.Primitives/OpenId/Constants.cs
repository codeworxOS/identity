namespace Codeworx.Identity.OpenId
{
    public static class Constants
    {
        public class Error : Identity.OAuth.Constants.Error
        {
            public const string InteractionRequired = "interaction_required";
            public const string LoginRequired = "login_required";
            public const string AccountSelectionRequired = "account_selection_required";
            public const string ConsentRequired = "consent_required";
            public const string InvalidRequestUri = "invalid_request_uri";
            public const string InvalidRequestObject = "invalid_request_object";
            public const string RequestNotSupported = "request_not_supported";
            public const string RequestUriNotSupported = "request_uri_not_supported";
            public const string RegistrationNotSupported = "registration_not_supported";
        }

        public class ResponseType : Identity.OAuth.Constants.ResponseType
        {
            public const string IdToken = "id_token";
        }

        public class Scopes
        {
            public const string OpenId = "openid";
        }

        public class ResponseMode
        {
            public const string FormPost = "form_post";
        }
    }
}