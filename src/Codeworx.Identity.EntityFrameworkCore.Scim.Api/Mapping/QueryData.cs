using System;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class QueryData
    {
        public QueryData(DbContext db, Guid providerId)
        {
            Db = db;
            ProviderId = providerId;
        }

        public DbContext Db { get; }

        public Guid ProviderId { get; }
    }
}
