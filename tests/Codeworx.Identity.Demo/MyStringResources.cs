using Codeworx.Identity.Resources;

public class MyStringResources : DefaultStringResources
{
    public override string GetResource(StringResource resource)
    {
        if (resource == StringResource.Password)
        {
            return "Kennwort";
        }

        return base.GetResource(resource);
    }
}