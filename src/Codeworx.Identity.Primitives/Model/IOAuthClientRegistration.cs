namespace Codeworx.Identity.Model
{
    public interface IOAuthClientRegistration
    {
        string Identifier { get; }

        string SupportedOAuthMode { get; }
    }
}
