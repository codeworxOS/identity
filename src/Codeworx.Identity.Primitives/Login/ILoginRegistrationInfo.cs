namespace Codeworx.Identity.Login
{
    public interface ILoginRegistrationInfo
    {
        string ProviderId { get; }

        bool HasRedirectUri(out string redirectUri);
    }
}
