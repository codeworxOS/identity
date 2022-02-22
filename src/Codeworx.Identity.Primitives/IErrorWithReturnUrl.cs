namespace Codeworx.Identity
{
    public interface IErrorWithReturnUrl : IEndUserErrorMessage
    {
        string ReturnUrl { get; }
    }
}
