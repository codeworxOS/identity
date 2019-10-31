namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginRegistration
    {
        string ProcessorKey { get; }

        string Name { get; }

        string Id { get; }

        object ProcessorConfiguration { get; }
    }
}