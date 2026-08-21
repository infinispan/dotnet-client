using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Infinispan.Hotrod
{
    public class InfinispanDistributedCacheOptions
    {
        public string CacheName { get; set; } = "default";

        public Action<InfinispanClient> ConfigureClient { get; set; }
    }

    public class InfinispanDistributedCache : IDistributedCache, IDisposable
    {
        private readonly InfinispanClient _client;
        private readonly Cache<string, byte[]> _cache;
        private readonly bool _ownsClient;

        public InfinispanDistributedCache(IOptions<InfinispanDistributedCacheOptions> optionsAccessor)
        {
            var options = optionsAccessor.Value;
            _client = new InfinispanClient();
            _ownsClient = true;
            options.ConfigureClient?.Invoke(_client);
            _cache = _client.NewCache(StringMarshaller._ASCII, ByteArrayMarshaller.Instance, options.CacheName);
        }

        public InfinispanDistributedCache(InfinispanClient client, string cacheName = "default")
        {
            _client = client;
            _ownsClient = false;
            _cache = _client.NewCache(StringMarshaller._ASCII, ByteArrayMarshaller.Instance, cacheName);
        }

        public byte[] Get(string key)
        {
            return GetAsync(key).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _cache.Get(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var (lifespan, maxIdle) = ConvertOptions(options);
            await _cache.Put(key, value, lifespan, maxIdle);
        }

        public void Refresh(string key)
        {
            RefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            // A Get triggers the server to reset the idle timer
            await _cache.Get(key);
        }

        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            await _cache.Remove(key);
        }

        private static (ExpirationTime lifespan, ExpirationTime maxIdle) ConvertOptions(DistributedCacheEntryOptions options)
        {
            ExpirationTime lifespan = null;
            ExpirationTime maxIdle = null;

            if (options == null)
                return (lifespan, maxIdle);

            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                lifespan = FromTimeSpan(options.AbsoluteExpirationRelativeToNow.Value);
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                var relative = options.AbsoluteExpiration.Value - DateTimeOffset.UtcNow;
                if (relative > TimeSpan.Zero)
                    lifespan = FromTimeSpan(relative);
            }

            if (options.SlidingExpiration.HasValue)
            {
                maxIdle = FromTimeSpan(options.SlidingExpiration.Value);
            }

            return (lifespan, maxIdle);
        }

        private static ExpirationTime FromTimeSpan(TimeSpan span)
        {
            long totalSeconds = (long)span.TotalSeconds;
            if (totalSeconds > 0)
                return new ExpirationTime { Unit = TimeUnit.SECONDS, Value = totalSeconds };
            return new ExpirationTime { Unit = TimeUnit.MILLISECONDS, Value = (long)span.TotalMilliseconds };
        }

        public void Dispose()
        {
            if (_ownsClient)
                _client.Dispose();
        }
    }
}
