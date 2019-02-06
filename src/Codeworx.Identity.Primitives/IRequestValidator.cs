namespace Codeworx.Identity
{
    public interface IRequestValidator<in TRequest, out TResult>
    {
        IValidationResult<TResult> IsValid(TRequest request);
    }
}
