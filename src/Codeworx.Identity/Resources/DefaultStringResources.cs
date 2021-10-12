namespace Codeworx.Identity.Resources
{
    public class DefaultStringResources : IStringResources
    {
        public virtual string GetResource(StringResource resource)
        {
            switch (resource)
            {
                case StringResource.Username:
                    return Translation.Username;
                case StringResource.Password:
                    return Translation.Password;
                default:
                    throw new MissingResourceException(resource);
            }
        }
    }
}
