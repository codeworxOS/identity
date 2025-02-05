﻿using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public interface IResourceMapping<TEntity, TResource, TData> : IResourceMapping<TEntity>
        where TEntity : class
        where TResource : IScimResource
    {
        Expression<Func<TResource, TData>> Resource { get; }

        Expression<Func<ScimEntity<TEntity>, TData>> Entity { get; }
    }
}
