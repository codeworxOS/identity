namespace Codeworx.Identity.Login
{
    public interface ILoginRegistrationInfo
    {
        string Template { get; }

        string Error { get; }

        string ProviderId { get; }

        bool HasRedirectUri(out string redirectUri);
    }
}
