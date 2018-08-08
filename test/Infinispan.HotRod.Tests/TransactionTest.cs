using Infinispan.HotRod.Config;
using Infinispan.HotRod.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Infinispan.HotRod.Tests.Util;


namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class TransactionTest
    {
        RemoteCacheManager remoteManager;
        RemoteCacheManager nonTxRemoteManager;

        IMarshaller marshaller;
        IMarshaller nonTxMarshaller;

        private void InitializeRemoteCacheManager(bool started)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000).Transactional(true);
            conf.ProtocolVersion("2.7");
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), started);
        }

        private void InitializeNonTxRemoteCacheManager(bool started)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.ProtocolVersion("2.7");
            nonTxMarshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            nonTxRemoteManager = new RemoteCacheManager(conf.Build(), started);
        }

        [Test]
        public void ReadCommitted()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            try
            {
                txManager.Begin();
                cache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                txManager.Commit();
            }
            catch (Exception ex)
            {
                // try to release the tx resources
                txManager.Rollback();
                throw ex;
            }
            // Check the correct value from remote cache
            rv1 = cache.Get(k1);
            Assert.AreEqual(rv1, v1);
        }

        [Test]
        public void ReadRollbackOnNotExistent()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            try
            {
                txManager.Begin();
                cache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
            }
            finally {
                txManager.Rollback();
            }
            // Check the correct value from remote cache
            rv1 = cache.Get(k1);
            Assert.IsNull(rv1);
        }

        // Client must read last value both during the tx (from the context)
        // and after the commit (from the cache)
        [Test]
        public void ChangeExistentAndCommit()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v0 = "carbon";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            cache.Put(k1, v0);
            try
            {
                txManager.Begin();
                cache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                txManager.Commit();
            }
            catch (Exception ex)
            {
                // try to release the tx resources
                txManager.Rollback();
                throw ex;
            }
            // Check the correct value from remote cache
            rv1 = cache.Get(k1);
            Assert.AreEqual(rv1, v1);
        }

        // Client must read last value during the tx (from the context)
        // and the old value from the cache after the rollback
        [Test]
        public void ReadRollbackOnExistent()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string oldv = "oxygen";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            string oldrv1 = cache.Put(k1, oldv);
            Assert.IsNull(oldrv1);
            try
            {
                txManager.Begin();
                oldrv1 = cache.Put(k1, v1);
                Assert.AreEqual(oldrv1, oldv);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
            } finally {
                txManager.Rollback();
            }
            // Check the correct value from remote cache
            rv1 = cache.Get(k1);
            Assert.AreEqual(oldrv1, oldv);
        }

        // Client must read last value during the tx (from the context)
        [Test]
        public void ReadInTransactionContext()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            try
            {
                txManager.Begin();

                string oldv1 = cache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                cache.Remove(k1);
                rv1 = cache.Get(k1);
                Assert.IsNull(rv1);
            } finally {
                txManager.Rollback();
            }
        }

        // TX Client must read last value during the tx (from the context)
        // NONTX client must read old value from the cache
        [Test]
        public void ReadCommittedAndNonTxRead()
        {
            InitializeRemoteCacheManager(true);
            InitializeNonTxRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            IRemoteCache<string, string> nonTxCache = nonTxRemoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string rv1;
            string nontxrv1;

            cache.Clear();
            try
            {
                txManager.Begin();

                cache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                nontxrv1 = nonTxCache.Get(k1);
                Assert.IsNull(nontxrv1);
                txManager.Commit();
            }
            catch (Exception ex)
            {
                // try to release the tx resources
                txManager.Rollback();
                throw ex;
            }
            // Check the correct value from remote cache
            rv1 = cache.Get(k1);
            Assert.AreEqual(rv1, v1);
            nontxrv1 = nonTxCache.Get(k1);
            Assert.AreEqual(nontxrv1, v1);
        }

        // NONTX client must put/get values from the cache
        [Test]
        public void PutAndReadWithNonTxCache()
        {
            InitializeRemoteCacheManager(true);
            InitializeNonTxRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            IRemoteCache<string, string> nonTxCache = nonTxRemoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string rv1;

            cache.Clear();
            try
            {
                txManager.Begin();
                string oldv1 = nonTxCache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = nonTxCache.Get(k1);
                Assert.AreEqual(rv1, v1);
                rv1 = cache.Remove(k1);
                rv1 = cache.Get(k1);
                Assert.IsNull(rv1);
            } finally {
                txManager.Rollback();
            }
            rv1 = nonTxCache.Get(k1);
            Assert.AreEqual(rv1, v1);
        }

        // NONTX client must put/get values from the cache
        [Test]
        public void RepeatableGetForTxClient()
        {
            InitializeRemoteCacheManager(true);
            InitializeNonTxRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            IRemoteCache<string, string> nonTxCache = nonTxRemoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string v2 = "helium";
            string rv1;

            cache.Clear();
            try
            {
                txManager.Begin();

                string oldv1 = nonTxCache.Put(k1, v1);
                // Check the correct value from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                // This goes to the server
                oldv1 = nonTxCache.Put(k1, v2);
                // But this values comes from the tx context
                rv1 = cache.Get(k1);
                Assert.AreEqual(rv1, v1);
                cache.Remove(k1);
                rv1 = cache.Get(k1);
                Assert.IsNull(rv1);
            } finally {
                txManager.Rollback();
            }
            rv1 = nonTxCache.Get(k1);
            Assert.AreEqual(rv1, v2);
        }
        // NONTX client must put/get values from the cache
        [Test]
        public void ConflictsAndFail()
        {
            InitializeRemoteCacheManager(true);
            InitializeNonTxRemoteCacheManager(true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>("non_xa", true);
            IRemoteCache<string, string> nonTxCache = nonTxRemoteManager.GetCache<string, string>("non_xa", true);
            var txManager = remoteManager.GetTransactionManager();

            string k1 = "key13";
            string v1 = "boron";
            string k2 = "key14";
            string v2 = "helium";
            string vx = "calcium";

            cache.Clear();
            try
            {
                txManager.Begin();

                string oldv1 = cache.Put(k1, v1);
                string oldv2 = cache.Put(k2, v2);
                // Check the correct value from the tx context
                string rv1 = nonTxCache.Put(k1, vx);
                Assert.IsNull(rv1);
                Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientRollbackException>(() =>
                {
                    txManager.Commit();
                });
            }
            catch (Exception ex)
            {
                // try to release the tx resources
                txManager.Rollback();
                throw ex;
            }
            Assert.AreEqual(cache.Get(k1), vx);
            Assert.IsNull(cache.Get(k2));
        }
    }
}
