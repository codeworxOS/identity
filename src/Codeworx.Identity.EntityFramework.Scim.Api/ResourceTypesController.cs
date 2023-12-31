using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("scim/ResourceTypes")]
    [AllowAnonymous]
    public class ResourceTypesController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ListResponse>> GetResourceTypesAsync([FromQuery] string filter)
        {
            if (filter != null)
            {
                // 403 - If a "filter" is provided, the service provider SHOULD respond with HTTP status
                // code 403(Forbidden) to ensure that clients cannot incorrectly assume that any matching
                // conditions specified in a filter are true.
                return Forbid();
            }

            List<ResourceTypeResponse> result = new List<ResourceTypeResponse>
            {
                GetUserResourceInfo(),
                GetGroupResourceInfo(),
            };

            await Task.Yield();

            return new ListResponse(result);
        }

        [HttpGet("{resource}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceTypeResponse>> GetResourceTypeAsync(string resource)
        {
            await Task.Yield();

            switch (resource)
            {
                case "User":
                    return GetUserResourceInfo();
                case "Group":
                    return GetGroupResourceInfo();
                default:
                    return NotFound();
            }
        }

        private ResourceTypeResponse GetUserResourceInfo()
        {
            ResourceTypeResponse resourceType;
            var resourceTypeUrl = this.Url.ActionLink(controller: "ResourceTypes", action: "GetResourceType", values: new { resource = "User" })!;
            var info = new ScimResponseInfo("User", resourceTypeUrl, null, null);

            var userUrl = this.Url.ActionLink(controller: "Users", action: "GetUsers")!;
            resourceType = new ResourceTypeResponse(info, new Models.Resources.ResourceTypeResource(userUrl, "User", ScimConstants.Schemas.User));
            return resourceType;
        }

        private ResourceTypeResponse GetGroupResourceInfo()
        {
            var resourceTypeUrl = this.Url.ActionLink(controller: "ResourceTypes", action: "GetResourceType", values: new { resource = "Group" })!;
            var info = new ScimResponseInfo("Group", resourceTypeUrl, null, null);

            var groupUrl = this.Url.ActionLink(controller: "Groups", action: "GetGroups")!;
            var resourceType = new ResourceTypeResponse(info, new Models.Resources.ResourceTypeResource(groupUrl, "Group", ScimConstants.Schemas.Group));
            return resourceType;
        }
    }
}
