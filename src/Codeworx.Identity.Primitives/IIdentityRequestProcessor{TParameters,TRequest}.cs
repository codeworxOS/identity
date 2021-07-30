using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IIdentityRequestProcessor<in TParameters, in TRequest>
        where TParameters : IIdentityDataParameters
    {
        int SortOrder { get; }

        Task ProcessAsync(IIdentityDataParametersBuilder<TParameters> builder, TRequest request);
    }
}
