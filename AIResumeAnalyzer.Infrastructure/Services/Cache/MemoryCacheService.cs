using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AIResumeAnalyzer.Infrastructure.Services.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration)
    {
        _memoryCache.Set(
            key,
            value,
            expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);

        return Task.CompletedTask;
    }
}