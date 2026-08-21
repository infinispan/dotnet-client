using System;
using Microsoft.Extensions.Caching.Distributed;
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
    }
}
