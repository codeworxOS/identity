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
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("scim/Schemas")]
    [AllowAnonymous]
    public class SchemasController : Controller
    {
        private IEnumerable<ISchemaExtension> _schemaExtensions;
        private IEnumerable<IResourceMapping<User>> _userMappings;
        private IEnumerable<IResourceMapping<Group>> _groupMappings;

        public SchemasController(IEnumerable<ISchemaExtension> schemaExtensions, IEnumerable<IResourceMapping<User>> userMappings, IEnumerable<IResourceMapping<Group>> groupMappings)
        {
            _schemaExtensions = schemaExtensions;
            _userMappings = userMappings;
            _groupMappings = groupMappings;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ListResponse>> GetSchemasAsync([FromQuery] string filter)
        {
            if (filter != null)
            {
                // 403 - If a "filter" is provided, the service provider SHOULD respond with HTTP status
                // code 403(Forbidden) to ensure that clients cannot incorrectly assume that any matching
                // conditions specified in a filter are true.
                return Forbid();
            }

            List<SchemaDataResponse> result = new List<SchemaDataResponse>
            {
                GetSchemaData(typeof(UserResource), ScimConstants.Schemas.User),
                GetSchemaData(typeof(GroupResource), ScimConstants.Schemas.Group),
            };

            foreach (var extension in _schemaExtensions)
            {
                GetSchemaData(extension.TargetType, extension.Schema);
            }

            await Task.Yield();

            return new ListResponse(result);
        }

        [HttpGet("{schema}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchemaDataResponse>> GetSchemaAsync(string schema)
        {
            await Task.Yield();

            switch (schema)
            {
                case ScimConstants.Schemas.User:
                    return GetSchemaData(typeof(UserResource), ScimConstants.Schemas.User);
                case ScimConstants.Schemas.Group:
                    return GetSchemaData(typeof(GroupResource), ScimConstants.Schemas.Group);
                default:
                    var data = _schemaExtensions.FirstOrDefault(d => d.Schema == schema);
                    if (data != null)
                    {
                        return GetSchemaData(data.TargetType, data.Schema);
                    }

                    return NotFound();
            }
        }

        private SchemaDataResponse GetSchemaData(Type type, string schema)
        {
            var schemaUrl = this.Url.ActionLink(controller: "Schemas", action: "GetSchema", values: new { schema = schema })!;
            var info = new ScimResponseInfo(schema, schemaUrl, null, null);

            List<SchemaDataAttributeResource> attributes = new List<SchemaDataAttributeResource>();
            foreach (var mapping in _userMappings)
            {
                if (mapping.ResourceExpression.Parameters.First().Type == type)
                {
                    AddAttribute(attributes, mapping.ResourceExpression);
                }
            }

            foreach (var mapping in _groupMappings)
            {
                if (mapping.ResourceExpression.Parameters.First().Type == type)
                {
                    AddAttribute(attributes, mapping.ResourceExpression);
                }
            }

            var schemaResponse = new SchemaDataResponse(info, new SchemaDataResource(type.Name, attributes));
            return schemaResponse;
        }

        private void AddAttribute(List<SchemaDataAttributeResource> attributes, LambdaExpression resourceExpression)
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

                    bool isRequired = false;
                    string mutable = "readWrite";
                    string returned = "default";
                    string uniqueness = "none";

                    currentAttributes.Add(new SchemaDataAttributeResource(entry.Member.Name, memberType, false, null, isRequired, null, false, mutable, returned, uniqueness, null));
                }
            }
        }
    }
}
