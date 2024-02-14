using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/ResourceTypes")]
    [Produces("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    [ScimError]
    public class ResourceTypesController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public async Task<ActionResult<ListResponse>> GetResourceTypesAsync([FromQuery] string filter, Guid providerId)
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
                GetUserResourceInfo(providerId),
                GetGroupResourceInfo(providerId),
            };

            await Task.Yield();

            return new ListResponse(result);
        }

        [HttpGet("{resource}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public async Task<ActionResult<ResourceTypeResponse>> GetResourceTypeAsync(string resource, Guid providerId)
        {
            await Task.Yield();

            switch (resource)
            {
                case "User":
                    return GetUserResourceInfo(providerId);
                case "Group":
                    return GetGroupResourceInfo(providerId);
                default:
                    return NotFound();
            }
        }

        private ResourceTypeResponse GetUserResourceInfo(Guid providerId)
        {
            ResourceTypeResponse resourceType;
            var resourceTypeUrl = this.Url.ActionLink(controller: "ResourceTypes", action: "GetResourceType", values: new { resource = "User", providerId = providerId })!;
            var info = new ScimResponseInfo("User", resourceTypeUrl, null, null);

            ////var userUrl = this.Url.ActionLink(controller: "Users", action: "GetUsers", values: new { providerId = providerId })!;
            var userUrl = "/Users";
            resourceType = new ResourceTypeResponse(info, new Models.Resources.ResourceTypeResource(userUrl, "User", ScimConstants.Schemas.User));
            return resourceType;
        }

        private ResourceTypeResponse GetGroupResourceInfo(Guid providerId)
        {
            var resourceTypeUrl = this.Url.ActionLink(controller: "ResourceTypes", action: "GetResourceType", values: new { resource = "Group", providerId = providerId })!;
            var info = new ScimResponseInfo("Group", resourceTypeUrl, null, null);

            ////var groupUrl = this.Url.ActionLink(controller: "Groups", action: "GetGroups", values: new { providerId = providerId })!;
            var groupUrl = "/Groups";
            var resourceType = new ResourceTypeResponse(info, new Models.Resources.ResourceTypeResource(groupUrl, "Group", ScimConstants.Schemas.Group));
            return resourceType;
        }
    }
}
