using System;
using JbHiFi.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace JbHiFi.Services
{
    public class InMemoryRateLimitTracker : IRateLimitTracker
    {
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private const int LIMIT = 5;
        private static readonly TimeSpan TIME_WINDOW = TimeSpan.FromHours(1);

        public bool IsLimitExceeded(string apiKey)
        {
            var cacheKey = GetCacheKey(apiKey);
            if (_cache.TryGetValue<RateLimitEntry>(cacheKey, out var entry))
            {
                return entry?.Count >= LIMIT;  
            }

            return false;
        }

        public void RegisterCall(string apiKey)
        {
            var cacheKey = GetCacheKey(apiKey);

            if (_cache.TryGetValue<RateLimitEntry>(cacheKey, out var entry))
            {
                if (entry != null)
                {
                    entry.Count++;
                    _cache.Set(cacheKey, entry, TIME_WINDOW);
                }
            }
            else
            {
                _cache.Set(cacheKey, new RateLimitEntry { Count = 1 }, TIME_WINDOW);
            }
        }

        private string GetCacheKey(string apiKey) => $"rate_limit_{apiKey}";

        private class RateLimitEntry
        {
            public int Count { get; set; }
        }
    }
}
