using System;

namespace Codeworx.Identity.Configuration
{
    public class CacheOptions
    {
        public CacheOptions()
        {
            this.CleanupInterval = TimeSpan.FromMinutes(10);
            this.RetentionPeriod = TimeSpan.FromDays(7);
        }

        public CacheOptions(CacheOptions copy)
        {
            this.RetentionPeriod = copy.RetentionPeriod;
            this.CleanupInterval = copy.CleanupInterval;
        }

        public TimeSpan CleanupInterval { get; set; }

        public TimeSpan RetentionPeriod { get; set; }
    }
}
