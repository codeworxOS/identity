using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Schemas")]
    [AllowAnonymous]
    public class SchemasController : Controller
    {
        private readonly IResourceMapper<User> _userMapper;
        private readonly IResourceMapper<Group> _groupMapper;
        private readonly IContextWrapper _context;
        private readonly IEnumerable<ISchemaExtension> _schemaExtensions;
        private readonly IEnumerable<IResourceMapping<User>> _userMappings;
        private readonly IEnumerable<IResourceMapping<Group>> _groupMappings;

        public SchemasController(
            IContextWrapper context,
            IEnumerable<ISchemaExtension> schemaExtensions,
            IEnumerable<IResourceMapping<User>> userMappings,
            IEnumerable<IResourceMapping<Group>> groupMappings,
            IResourceMapper<User> userMapper,
            IResourceMapper<Group> groupMapper)
        {
            _context = context;
            _schemaExtensions = schemaExtensions;
            _userMappings = userMappings;
            _groupMappings = groupMappings;
            _userMapper = userMapper;
            _groupMapper = groupMapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        private void AddAttribute(List<SchemaDataAttributeResource> attributes, LambdaExpression resourceExpression, LambdaExpression entityExpression, string? shadowProperty = null, bool readOnly = false)
        {
            var paths = new List<MemberExpression>();

            var current = resourceExpression.Body;
            while (current != resourceExpression.Parameters[0] && current != null)
            {
                if (current is MemberExpression member)
                {
                    paths.Add(member);
                    current = member.Expression;
                }
                else
                {
                    throw new NotSupportedException("Only member expressions are supported e.g. (p.Username or p.Name.FirstName)");
                }
            }

            if (current == null)
            {
                throw new NotSupportedException("The root expression must be the resource parameter.");
            }

            paths.Reverse();

            List<SchemaDataAttributeResource> currentAttributes = attributes;
            foreach (var entry in paths)
            {
                if (entry != paths.Last())
                {
                    var existingAttribute = currentAttributes.FirstOrDefault(d => d.Name == entry.Member.Name);
                    if (existingAttribute == null)
                    {
                        existingAttribute = new SchemaDataAttributeResource(entry.Member.Name, false, null, false, false);
                        currentAttributes!.Add(existingAttribute);
                    }

                    currentAttributes = existingAttribute.SubAttributes!;
                }
                else if (entry.Member is PropertyInfo prop)
                {
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    var memberType = "string";
                    if (type == typeof(int) || type == typeof(uint) || type == typeof(ushort) || type == typeof(short) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong))
                    {
                        memberType = "integer";
                    }
                    else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                    {
                        memberType = "decimal";
                    }
                    else if (type == typeof(bool))
                    {
                        memberType = "boolean";
                    }
                    else if (type == typeof(DateTime))
                    {
                        memberType = "dateTime";
                    }
                    else if (type == typeof(string))
                    {
                        memberType = "string";
                    }

                    var entityInfo = GetEntityPropertyDetails(entityExpression, shadowProperty);
                    string mutable = readOnly ? "readOnly" : "readWrite";
                    string returned = "default";

                    currentAttributes.Add(new SchemaDataAttributeResource(entry.Member.Name, memberType, false, null, entityInfo.Required, null, false, mutable, returned, entityInfo.Unique ? "global" : "none", null));
                }
            }
        }

        private (bool Required, bool Unique) GetEntityPropertyDetails(LambdaExpression entityExpression, string? propertyName = null)
        {
            var type = _context.Context.Model.FindEntityType(entityExpression.Parameters[0].Type);
            if (type == null)
            {
                throw new NotSupportedException("EntityType does not exist in model!");
            }

            IProperty? property = null;

            if (propertyName != null)
            {
                property = type!.FindProperty(propertyName);
            }
            else
            {
                var member = entityExpression.Body as MemberExpression;

                if (member == null)
                {
                    throw new NotSupportedException("Only member expressions are supported e.g. (p.Username or p.Name.FirstName)");
                }

                property = type.FindProperty(member.Member);
            }

            if (property == null)
            {
                throw new NotSupportedException("Property not found!");
            }

            return (!property.IsNullable, property.IsUniqueIndex() || (type.FindIndex(property)?.IsUnique ?? false));
        }
    }
}
