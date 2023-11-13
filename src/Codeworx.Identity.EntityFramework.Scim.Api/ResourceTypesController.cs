using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("authProvider/{authProviderId}/scim/ResourceTypes")]
    [Authorize(Policy = "api_none")]
    public class ResourceTypesController
    {
        public ResourceTypesController()
        {
        }

        [HttpGet]
        public async Task<ListResponse<ResourceTypeResponse>> GetResourceTypesAsync([FromQuery] string filter)
        {
            if (filter != null)
            {
                // 403 - If a "filter" is provided, the service provider SHOULD respond with HTTP status
                // code 403(Forbidden) to ensure that clients cannot incorrectly assume that any matching
                // conditions specified in a filter are true.
                throw new NotSupportedException();
            }

            List<ResourceTypeResponse> result = new List<ResourceTypeResponse>
            {
                await GetResourceTypeAsync("User"),
                await GetResourceTypeAsync("Group"),
            };

            return new ListResponse<ResourceTypeResponse>(result);
        }

        [HttpGet("{resource}")]
        public Task<ResourceTypeResponse> GetResourceTypeAsync(string resource)
        {
            ResourceTypeResponse resourceType;
            switch (resource)
            {
                case "User":
                    resourceType = new ResourceTypeResponse()
                    {
                        Id = "User",
                        Name = "User",
                        EndPoint = "/Users",
                        Schema = SchemaConstants.User,
                        Meta = new MetaData
                        {
                            Location = "scim/ResourceTypes/User",
                            ResourceType = "ResourceType",
                        },
                    };
                    break;
                case "Group":
                    resourceType = new ResourceTypeResponse()
                    {
                        Id = "Group",
                        Name = "Group",
                        EndPoint = "/Groups",
                        Schema = SchemaConstants.Group,
                        Meta = new MetaData
                        {
                            Location = "scim/ResourceTypes/Group",
                            ResourceType = "ResourceType",
                        },
                    };
                    break;
                default:
                    // 404
                    throw new NotSupportedException();
            }

            return Task.FromResult(resourceType);
        }
    }
}
