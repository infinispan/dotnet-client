using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Org.Infinispan.Query.Remote.Client;

namespace Infinispan.Hotrod.Linq
{
    public static class CacheQueryExtensions
    {
        public static IQueryable<V> AsQueryable<K, V>(this Cache<K, V> cache)
            where V : IMessage<V>, new()
        {
            var provider = new IckleQueryProvider(
                qr => cache.Query(qr),
                bytes => cache.ValueMarshaller.Unmarshall(bytes));
            return new IckleQueryable<V>(provider);
        }
    }
}
