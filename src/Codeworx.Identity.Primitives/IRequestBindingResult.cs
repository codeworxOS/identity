namespace Codeworx.Identity
{
    public interface IRequestBindingResult<out TResult, out TError>
    {
        TResult Result { get; }

        TError Error { get; }
    }
}