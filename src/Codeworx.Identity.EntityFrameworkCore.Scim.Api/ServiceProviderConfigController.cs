using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/ServiceProviderConfig")]
    [Produces("application/scim+json", "application/json")]
    [Consumes("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    [ScimError]
    public class ServiceProviderConfigController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public Task<ServiceProviderConfigResponse> GetAsync()
        {
            return Task.FromResult(new ServiceProviderConfigResponse());
        }
    }
}
