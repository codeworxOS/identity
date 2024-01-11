using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/ServiceProviderConfig")]
    [Produces("application/scim+json", "application/json")]
    [Consumes("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    public class ServiceProviderConfigController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ServiceProviderConfigResponse> GetAsync()
        {
            return Task.FromResult(new ServiceProviderConfigResponse());
        }
    }
}
