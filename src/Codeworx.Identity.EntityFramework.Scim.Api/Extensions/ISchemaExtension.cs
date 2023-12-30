using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public interface ISchemaExtension
    {
        string Schema { get; }

        Type TargetType { get; }
    }
}
