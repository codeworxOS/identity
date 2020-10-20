using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OpenId
{
    public class ScopeIdentityDataRequestProcessor : IIdentityRequestProcessor<IIdentityDataParameters, object>
    {
        private readonly IScopeService _scopeService;

        public ScopeIdentityDataRequestProcessor(
            IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public int SortOrder => 310;

        public Task ProcessAsync(IIdentityDataParametersBuilder<IIdentityDataParameters> builder, object request)
        {
            if (!builder.Parameters.Scopes.Contains(Constants.OpenId.Scopes.OpenId))
            {
                builder.Throw(Constants.OAuth.Error.InvalidScope, $"The {Constants.OpenId.Scopes.OpenId} scope is missing.");
            }

            return Task.CompletedTask;
        }
    }
}
