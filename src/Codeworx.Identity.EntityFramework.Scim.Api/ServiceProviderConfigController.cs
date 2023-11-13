using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("authProvider/{authProviderId}/scim/ServiceProviderConfig")]
    [Authorize(Policy = "api_none")]
    public class ServiceProviderConfigController
    {
        public ServiceProviderConfigController()
        {
        }

        [HttpGet]
        public Task<ServiceProviderConfigResponse> GetAsync()
        {
            return Task.FromResult(new ServiceProviderConfigResponse());
        }
    }
}
