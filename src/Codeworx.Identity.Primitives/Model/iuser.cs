namespace Codeworx.Identity.Model
{
    public interface IUser
    {
        string Identity { get; }

        string Name { get; }
    }
}