using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public interface IUserSchemaExtension
    {
        string Schema { get; }

        Type TargetType { get; }
    }
}
