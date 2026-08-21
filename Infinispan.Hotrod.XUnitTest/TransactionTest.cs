using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class TransactionTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public Cache<string, string> cache;
        public Cache<string, string> txCache;
        public InfinispanClient infinispan = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = 0x1f;
            infinispan.ForceReturnValue = false;
            infinispan.ClientIntelligence = 0x01;
            cache = infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
            txCache = infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "transactional");
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class TransactionTest : IClassFixture<TransactionTestFixture>
    {
        private readonly Cache<string, string> _cache;
        private readonly Cache<string, string> _txCache;

        public TransactionTest(TransactionTestFixture fixture)
        {
            _cache = fixture.cache;
            _txCache = fixture.txCache;
        }

        [Fact]
        public async Task TransactionCommitTest()
        {
            string key = UniqueKey.NextKey();
            await _txCache.Clear();

            var tx = _txCache.BeginTransaction();
            await tx.Put(key, "value1");
            await tx.CommitAsync();

            Assert.Equal("value1", await _txCache.Get(key));
        }

        [Fact]
        public async Task TransactionReadThenWriteTest()
        {
            string key = UniqueKey.NextKey();
            await _txCache.Put(key, "original");

            var tx = _txCache.BeginTransaction();
            var val = await tx.Get(key);
            Assert.Equal("original", val);

            await tx.Put(key, "modified");
            await tx.CommitAsync();

            Assert.Equal("modified", await _txCache.Get(key));
        }

        [Fact]
        public async Task TransactionRemoveTest()
        {
            string key = UniqueKey.NextKey();
            await _txCache.Put(key, "toRemove");

            var tx = _txCache.BeginTransaction();
            await tx.Remove(key);
            await tx.CommitAsync();

            Assert.Null(await _txCache.Get(key));
        }

        [Fact]
        public async Task TransactionBlindWriteTest()
        {
            string key = UniqueKey.NextKey();
            await _txCache.Clear();

            var tx = _txCache.BeginTransaction();
            tx.PutBlind(key, "blind-value");
            await tx.CommitAsync();

            Assert.Equal("blind-value", await _txCache.Get(key));
        }

        [Fact]
        public async Task TransactionBufferingTest()
        {
            string key = UniqueKey.NextKey();
            await _txCache.Clear();

            var tx = _txCache.BeginTransaction();
            await tx.Put(key, "v1");
            var val = await tx.Get(key);
            Assert.Equal("v1", val);

            await tx.Put(key, "v2");
            val = await tx.Get(key);
            Assert.Equal("v2", val);

            await tx.CommitAsync();
            Assert.Equal("v2", await _txCache.Get(key));
        }

        [Fact]
        public async Task TransactionMultipleKeysTest()
        {
            string key1 = UniqueKey.NextKey();
            string key2 = UniqueKey.NextKey();
            string key3 = UniqueKey.NextKey();
            await _txCache.Clear();

            var tx = _txCache.BeginTransaction();
            await tx.Put(key1, "a");
            await tx.Put(key2, "b");
            await tx.Put(key3, "c");
            await tx.CommitAsync();

            Assert.Equal("a", await _txCache.Get(key1));
            Assert.Equal("b", await _txCache.Get(key2));
            Assert.Equal("c", await _txCache.Get(key3));
        }

        [Fact]
        public async Task TransactionDoubleCommitThrowsTest()
        {
            var tx = _txCache.BeginTransaction();
            tx.PutBlind(UniqueKey.NextKey(), "v");
            await tx.CommitAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => tx.CommitAsync());
        }

        [Fact]
        public async Task EmptyTransactionCommitTest()
        {
            var tx = _txCache.BeginTransaction();
            var code = await tx.CommitAsync();
            Assert.Equal(XaReturnCode.XA_RDONLY, code);
        }
    }
}
