namespace Codeworx.Identity
{
    public interface IValidationResult<out TResult>
    {
        TResult GetError();
    }
}