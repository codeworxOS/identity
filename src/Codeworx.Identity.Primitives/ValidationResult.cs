namespace Codeworx.Identity
{
    public abstract class ValidationResult<TResult> : IValidationResult<TResult>
    {
        public abstract TResult GetError();
    }
}