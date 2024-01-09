using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public interface ISchemaExtension
    {
        string Schema { get; }

        string Name { get; }

        Type TargetType { get; }
    }
}
