using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infinispan.Hotrod
{
    public static class InfinispanCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddInfinispanCache(this IServiceCollection services, Action<InfinispanDistributedCacheOptions> setupAction)
        {
            services.Configure(setupAction);
            services.TryAddSingleton<IDistributedCache, InfinispanDistributedCache>();
            return services;
        }

        public static IServiceCollection AddInfinispanHybridCache(this IServiceCollection services, Action<InfinispanDistributedCacheOptions> setupAction, Action<HybridCacheOptions> hybridCacheSetup = null)
        {
            services.AddInfinispanCache(setupAction);
            if (hybridCacheSetup != null)
                services.AddHybridCache(hybridCacheSetup);
            else
                services.AddHybridCache();
            return services;
        }
    }
}
