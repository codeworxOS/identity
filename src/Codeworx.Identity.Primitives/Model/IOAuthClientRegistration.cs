namespace Codeworx.Identity.Model
{
    public interface IOAuthClientRegistration
    {
        string TenantIdentifier { get; }

        string Identifier { get; }

        string SupportedOAuthMode { get; }
    }
}
