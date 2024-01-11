using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public interface IGroupSchemaProperty
    {
        public Type ResourceType { get; }

        string EntityPropertyName { get; }

        object? GetResourceValue(object resource);

        void SetResourceValue(object resource, object? value);
    }
}
