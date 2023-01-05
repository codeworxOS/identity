using System.Runtime.Serialization;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    public class WellKnownResponse
    {
        [DataMember(Order = 1, Name = "issuer")]
        public string Issuer { get; set; }

        [DataMember(Order = 2, Name = "token_endpoint")]
        public string TokenEndpoint { get; set; }

        [DataMember(Order = 3, Name = "introspection_endpoint")]
        public string IntrospectionEndpoint { get; set; }

        [DataMember(Order = 5, Name = "authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [DataMember(Order = 6, Name = "userinfo_endpoint")]
        public string UserInfoEndpoint { get; set; }

        [DataMember(Order = 7, Name = "registration_endpoint")]
        public string ClientRegistrationEndpoint { get; set; }

        [DataMember(Order = 8, Name = "jwks_uri")]
        public string JsonWebKeyEndpoint { get; set; }

        [DataMember(Order = 9, Name = "scopes_supported")]
        public string[] SupportedScopes { get; set; }

        [DataMember(Order = 10, Name = "response_types_supported")]
        public string[] SupportedResponseTypes { get; set; }

        [DataMember(Order = 12, Name = "subject_types_supported")]
        public string[] SupportedSubjectTypes { get; set; }

        [DataMember(Order = 13, Name = "id_token_signing_alg_values_supported")]
        public string[] SupportedIdTokenSigningAlgorithms { get; set; }

        [DataMember(Order = 25, Name = "claims_supported")]
        public string[] SupportedClaims { get; set; }
    }
}