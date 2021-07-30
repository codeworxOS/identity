namespace Codeworx.Identity
{
    public interface IIdentityDataParametersBuilder<out TParameters>
        where TParameters : IIdentityDataParameters
    {
        TParameters Parameters { get; }

        void SetValue(string property, object value);

        void Throw(string error, string errorDescription);
    }
}
