﻿namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public interface ISchemaResource : IScimResource
    {
        string Schema { get; }
    }
}
