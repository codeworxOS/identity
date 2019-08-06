namespace Codeworx.Identity.Model
{
    public interface ISupportedFlow
    {
        bool IsSupported(string flowKey);
    }
}
