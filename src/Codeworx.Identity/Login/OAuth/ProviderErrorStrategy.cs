namespace Codeworx.Identity.Login.OAuth
{
    public enum ProviderErrorStrategy
    {
        None = 0x00,
        AppendLoginPrompt = 0x01,
        AppendSelectAccountPrompt = 0x02,
    }
}