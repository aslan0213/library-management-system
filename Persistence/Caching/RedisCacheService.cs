using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Abstractions.Services;
using Microsoft.Extensions.Caching.Distributed;
namespace Persistence.Caching
{
	public class RedisCacheService : ICacheService
	{
		public readonly IDistributedCache _cache;
		public RedisCacheService(IDistributedCache cache)
		{
			_cache = cache;
		}
		public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			var data = await _cache.GetAsync(key, cancellationToken);
			return data is null ? default : JsonSerializer.Deserialize<T>(data);
		}

		public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
		{
			await _cache.RemoveAsync(key, cancellationToken);
		}

		public async Task SetAsync<T>(string key, T? value, TimeSpan expiration, CancellationToken cancellationToken = default)
		{
			var data = JsonSerializer.Serialize(value);
			var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
			await _cache.SetStringAsync(key, data, options, cancellationToken);
		}
	}
}
