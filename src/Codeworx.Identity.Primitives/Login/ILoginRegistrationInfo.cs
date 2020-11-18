namespace Codeworx.Identity.Login
{
    public interface ILoginRegistrationInfo
    {
        string Error { get; }

        string ProviderId { get; }

        bool HasRedirectUri(out string redirectUri);
    }
}
