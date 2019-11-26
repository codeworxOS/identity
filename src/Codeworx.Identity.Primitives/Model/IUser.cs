namespace Codeworx.Identity.Model
{
    public interface IUser
    {
        string DefaultTenantKey { get; }

        string Identity { get; }

        string Name { get; }

        byte[] PasswordHash { get; }

        byte[] PasswordSalt { get; }
    }
}