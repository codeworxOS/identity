using Codeworx.Identity.View;

public class MyPasswordChangeViewTemplate : IPasswordChangeViewTemplate
{
    public Task<string> GetPasswordChangeTemplate()
    {
        return File.ReadAllTextAsync("c:\\Temp\\change-password.html");
    }
}