namespace Codeworx.Identity.Model
{
    public interface IStringPolicy
    {
        bool IsValid(string value, string languageCode, out string validationMessage);
    }
}
