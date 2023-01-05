using System;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class CacheEntry<TData>
        where TData : class
    {
        public CacheEntry(TData data, DateTime validUntil)
        {
            Data = data;
            ValidUntil = validUntil;
        }

        public TData Data { get; }

        public DateTime ValidUntil { get; }
    }
}