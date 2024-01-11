using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Schemas")]
    [Produces("application/scim+json", "application/json")]
    [Consumes("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    [ScimError]
    public class SchemasController : Controller
    {
        private readonly IResourceMapper<User> _userMapper;
        private readonly IResourceMapper<Group> _groupMapper;
        private readonly IContextWrapper _context;

        public SchemasController(
            IContextWrapper context,
            IResourceMapper<User> userMapper,
            IResourceMapper<Group> groupMapper)
        {
            _context = context;
            _userMapper = userMapper;
            _groupMapper = groupMapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public async Task<ActionResult<ListResponse>> GetSchemasAsync([FromQuery] string filter, Guid providerId)
        {
            if (filter != null)
            {
                // 403 - If a "filter" is provided, the service provider SHOULD respond with HTTP status
                // code 403(Forbidden) to ensure that clients cannot incorrectly assume that any matching
                // conditions specified in a filter are true.
                return Forbid();
            }

            var schemas = new List<SchemaDataResource>();

            await foreach (var item in _userMapper.GetSchemasAsync(_context.Context))
            {
                schemas.Add(item);
            }

            await foreach (var item in _groupMapper.GetSchemasAsync(_context.Context))
            {
                schemas.Add(item);
            }

            var result = schemas.Select(p => GetSchemaData(p, providerId)).ToList();

            return new ListResponse(result);
        }

        [HttpGet("{schema}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public async Task<ActionResult<SchemaDataResponse>> GetSchemaAsync(string schema, Guid providerId)
        {
            await Task.Yield();

            SchemaDataResource? found = null;

            await foreach (var item in _userMapper.GetSchemasAsync(_context.Context))
            {
                if (item.Id == schema)
                {
                    found = item;
                    break;
                }
            }

            if (found == null)
            {
                await foreach (var item in _groupMapper.GetSchemasAsync(_context.Context))
                {
                    if (item.Id == schema)
                    {
                        found = item;
                        break;
                    }
                }
            }

            if (found != null)
            {
                return GetSchemaData(found, providerId);
            }

            return NotFound();
        }

        private SchemaDataResponse GetSchemaData(SchemaDataResource resource, Guid providerId)
        {
            var schemaUrl = this.Url.ActionLink(controller: "Schemas", action: "GetSchema", values: new { schema = resource.Id, providerId = providerId })!;
            var info = new ScimResponseInfo(resource.Id, schemaUrl, null, null);
            var schemaResponse = new SchemaDataResponse(info, resource);
            return schemaResponse;
        }
    }
}
