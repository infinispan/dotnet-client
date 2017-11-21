using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinispan.HotRod.Tests
{
    public abstract class BaseAuthorizationTest
    {
        public const string HOTROD_HOST = "127.0.0.1";
        public const int HOTROD_PORT = 11222;
        public const string AUTH_CACHE = "authCache";
        public const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        public const string REALM = "ApplicationRealm";
        public const string K1 = "k1";
        public const string V1 = "v1";
        public const string K2 = "k2";
        public const string V2 = "v2";
        public const string NON_EXISTENT_KEY = "nonExistentKey";

        protected IRemoteCache<String, String> readerCache;
        protected IRemoteCache<String, String> writerCache;
        protected IRemoteCache<String, String> supervisorCache;
        protected IRemoteCache<String, String> adminCache;
        protected IRemoteCache<String, String> scriptCache;

        protected IMarshaller marshaller;

        public abstract string GetMech();

        public virtual string GetServerName()
        {
            return "node0";
        }

        [TestFixtureSetUp]
        public void BeforeClass()
        {
            readerCache = InitCache("reader", "password");
            writerCache = InitCache("writer", "somePassword");
            supervisorCache = InitCache("supervisor", "lessStrongPassword");
            adminCache = InitCache("admin", "strongPassword");
            scriptCache = InitCache("admin", "strongPassword", PROTOBUF_SCRIPT_CACHE_NAME);
        }

        protected IRemoteCache<String, String> InitCache(string user, string password, string cacheName = AUTH_CACHE)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer()
                    .Host(HOTROD_HOST)
                    .Port(HOTROD_PORT)
                    .ConnectionTimeout(90000)
                    .SocketTimeout(900);
            conf.Security().Authentication()
                                .Enable()
                                .SaslMechanism(GetMech())
                                .ServerFQDN(GetServerName())
                                .SetupCallback(user, password, REALM);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> authCache = remoteManager.GetCache<string, string>(cacheName);
            return authCache;
        }

        [Test]
        public void ReaderSuccessTest()
        {
            TestContainsKey(readerCache);
            TestGetNonExistent(readerCache);
            TestGetVersioned(readerCache);
            TestGetWithMetadata(readerCache);
        }

        [Test]
        public void ReaderPerformsWritesTest()
        {
            AssertError(readerCache, cache => TestPut(cache));
            AssertError(readerCache, cache => TestPutAsync(cache));
            AssertError(readerCache, cache => TestRemoveNonExistent(cache));
            AssertError(readerCache, cache => TestRemoveAsyncNonExistent(cache));
        }

        [Test]
        public void WriterSuccessTest()
        {
            TestPut(writerCache);
            TestPutAsync(writerCache);
            TestRemoveNonExistent(writerCache);
            TestRemoveAsyncNonExistent(writerCache);
        }

        [Test]
        public void WriterPerformsReadsTest()
        {
            AssertError(writerCache, cache => TestContainsKey(cache));
            AssertError(writerCache, cache => TestGetNonExistent(cache));
            AssertError(writerCache, cache => TestGetVersioned(cache));
            AssertError(writerCache, cache => TestGetWithMetadata(cache));
        }

        [Test]
        public void WriterPerformsSupervisorOpsTest()
        {
            AssertError(writerCache, cache => TestPutClear(cache));
            AssertError(writerCache, cache => TestPutContains(cache));
            AssertError(writerCache, cache => TestPutGetBulk(cache));
            AssertError(writerCache, cache => TestPutGetVersioned(cache));
            AssertError(writerCache, cache => TestPutGetWithMetadata(cache));
            AssertError(writerCache, cache => TestPutAll(cache));
            AssertError(writerCache, cache => TestPutIfAbsent(cache));
            AssertError(writerCache, cache => TestPutRemoveContains(cache));
            AssertError(writerCache, cache => TestPutRemoveWithVersion(cache));
            AssertError(writerCache, cache => TestPutReplaceWithFlag(cache));
            AssertError(writerCache, cache => TestPutReplaceWithVersion(cache));
            AssertError(writerCache, cache => TestPutSize(cache));
            AssertError(writerCache, cache => TestRemoteTaskExec(cache));
        }

        [Test]
        public void SupervisorSuccessTest()
        {
            TestCommonSupervisorAdminOps(supervisorCache);
        }

        [Test]
        public void SupervisorPerformsAdminOpsTest()
        {
            AssertError(supervisorCache, cache => TestStats(cache));
            AssertError(supervisorCache, cache => TestAddRemoveListener(cache));
        }

        [Test]
        public void AdminSuccessTest()
        {
            TestCommonSupervisorAdminOps(adminCache);
            TestStats(adminCache);
            TestAddRemoveListener(adminCache);
            TestPutKeySet(adminCache);
        }

        private void TestCommonSupervisorAdminOps(IRemoteCache<string, string> cache)
        {
            TestPutClear(cache);
            TestPutClearAsync(cache);
            TestPutContains(cache);
            TestPutGet(cache);
            TestPutGetAsync(cache);
            TestPutGetBulk(cache);
            TestPutGetVersioned(cache);
            TestPutGetWithMetadata(cache);
            TestPutAll(cache);
            TestPutAllAsync(cache);
            TestPutIfAbsent(cache); //requires both READ and WRITE permissions
            TestPutIfAbsentAsync(cache);
            TestPutRemoveContains(cache);
            TestPutRemoveAsyncContains(cache);
            TestPutRemoveWithVersion(cache);
            TestPutRemoveWithVersionAsync(cache);
            TestPutReplaceWithFlag(cache);
            TestPutReplaceWithVersion(cache);
            TestPutReplaceWithVersionAsync(cache);
            TestPutSize(cache);
            TestRemoteTaskExec(cache);
            //see ISPN-8059 - test this only for Admin user
            //TestPutKeySet(cache);
        }

        private void AssertError(IRemoteCache<String, String> cache, Action<IRemoteCache<String, String>> f)
        {
            const string ERROR_MSG = "ERROR: Unauthorized operation performed!";
            try
            {
                f.Invoke(cache);
                Assert.Fail(ERROR_MSG);
            }
            catch (Infinispan.HotRod.Exceptions.HotRodClientException) { }
            catch (AggregateException ag)
            {
                foreach (Exception ex in ag.InnerExceptions)
                {
                    Assert.AreEqual(typeof(Infinispan.HotRod.Exceptions.HotRodClientException),
                                    ex.GetType());
                }
            }
        }

        protected void TestContainsKey(IRemoteCache<string, string> cache)
        {
            Assert.IsFalse(cache.ContainsKey(NON_EXISTENT_KEY));
        }

        protected void TestGetNonExistent(IRemoteCache<string, string> cache)
        {
            Assert.IsNull(cache.Get(NON_EXISTENT_KEY));
        }

        protected void TestGetVersioned(IRemoteCache<string, string> cache)
        {
            Assert.IsNull(cache.GetVersioned(NON_EXISTENT_KEY));
        }

        protected void TestGetWithMetadata(IRemoteCache<string, string> cache)
        {
            Assert.IsNull(cache.GetWithMetadata(NON_EXISTENT_KEY));
        }

        protected void TestPut(IRemoteCache<string, string> cache)
        {
            Assert.IsNull(cache.Put(K1, V1));
        }

        protected void TestPutAsync(IRemoteCache<string, string> cache)
        {
            Task<string> resultAsync = cache.PutAsync(K1, V1);
            Assert.IsNull(resultAsync.Result);
        }

        protected void TestRemoveNonExistent(IRemoteCache<string, string> cache)
        {
            Assert.IsNull(cache.Remove(NON_EXISTENT_KEY));
        }

        protected void TestRemoveAsyncNonExistent(IRemoteCache<string, string> cache)
        {
            Task<string> removeAsync = cache.RemoveAsync(NON_EXISTENT_KEY);
            Assert.IsNull(removeAsync.Result);
        }

        protected void TestPutClear(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            cache.Put(K2, V2);
            cache.Clear();
            Assert.IsTrue(cache.IsEmpty());
        }

        protected void TestPutClearAsync(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            cache.Put(K2, V2);
            Task task = cache.ClearAsync();
            task.Wait(5000);
            Assert.IsTrue(cache.IsEmpty());
        }

        protected void TestPutContains(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.IsTrue(cache.ContainsKey(K1));
        }

        protected void TestPutGet(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.AreEqual(V1, cache.Get(K1));
        }

        protected void TestPutGetAsync(IRemoteCache<string, string> cache)
        {
            Task<string> putAsync = cache.PutAsync(K1, V1);
            Assert.IsNull(putAsync.Result);
            Task<string> getAsync = cache.GetAsync(K1);
            Assert.AreEqual(V1, getAsync.Result);
        }

        protected void TestPutGetBulk(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            cache.Remove(K2);
            ulong before = cache.Size();
            cache.Put(K1, V1);
            cache.Put(K2, V2);
            Assert.AreEqual(before + 2, cache.GetBulk().Count);
        }

        protected void TestPutGetVersioned(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            IVersionedValue<string> value = cache.GetVersioned(K1);
            Assert.AreEqual(V1, value.GetValue());
            Assert.AreNotEqual(0, value.GetVersion());
        }

        protected void TestPutGetWithMetadata(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.NotNull(cache.GetWithMetadata(K1));
        }

        protected void TestPutAll(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            cache.Remove(K2);
            ulong before = cache.Size();
            IDictionary<string, string> entries = new Dictionary<string, string>();
            entries.Add(K1, V1);
            entries.Add(K2, V2);
            cache.PutAll(entries);
            Assert.AreEqual(before + 2, cache.Size());
        }

        protected void TestPutAllAsync(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            cache.Remove(K2);
            ulong before = cache.Size();
            IDictionary<string, string> entries = new Dictionary<string, string>();
            entries.Add(K1, V1);
            entries.Add(K2, V2);
            Task result = cache.PutAllAsync(entries);
            result.Wait(5000);
            Assert.AreEqual(before + 2, cache.Size());
        }

        protected void TestPutIfAbsent(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            Assert.IsNull(cache.PutIfAbsent(K1, V1));
            //this should not change the value
            cache.PutIfAbsent(K1, V2);
            Assert.AreEqual(V1, cache.Get(K1));
        }

        protected void TestPutIfAbsentAsync(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            Task<string> result = cache.PutIfAbsentAsync(K1, V1);
            Assert.IsNull(result.Result);
            //this should not change the value
            result = cache.PutIfAbsentAsync(K1, V2);
            Assert.AreEqual(V1, cache.Get(K1));
        }

        protected void TestPutRemoveContains(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.IsTrue(cache.ContainsKey(K1));
            cache.Remove(K1);
            Assert.IsFalse(cache.ContainsKey(K1));
        }

        protected void TestPutRemoveAsyncContains(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.IsTrue(cache.ContainsKey(K1));
            Task<string> result = cache.RemoveAsync(K1);
            result.Wait(5000);
            Assert.IsFalse(cache.ContainsKey(K1));
        }

        protected void TestPutRemoveWithVersion(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            IVersionedValue<string> value = cache.GetVersioned(K1);
            ulong version = value.GetVersion();
            cache.RemoveWithVersion(K1, version);
            value = cache.GetVersioned(K1);
            if (value != null)
            {
                Assert.AreNotEqual(value.GetVersion(), version);
            }
        }

        protected void TestPutRemoveWithVersionAsync(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            IVersionedValue<string> value = cache.GetVersioned(K1);
            ulong version = value.GetVersion();
            Task<bool> result = cache.RemoveWithVersionAsync(K1, version);
            result.Wait(5000);
            value = cache.GetVersioned(K1);
            if (value != null)
            {
                Assert.AreNotEqual(value.GetVersion(), version);
            }
        }

        protected void TestPutReplaceWithFlag(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.AreEqual(V1, cache.WithFlags(Flags.FORCE_RETURN_VALUE).Replace(K1, V2));
            Assert.AreEqual(V2, cache.Get(K1));
        }

        protected void TestPutReplaceWithVersion(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            IVersionedValue<string> value = cache.GetVersioned(K1);
            ulong version = value.GetVersion();
            cache.ReplaceWithVersion(K1, V2, version);
            value = cache.GetVersioned(K1);
            Assert.AreEqual(V2, value.GetValue());
            Assert.IsTrue(value.GetVersion() != version);
        }

        protected void TestPutReplaceWithVersionAsync(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            IVersionedValue<string> value = cache.GetVersioned(K1);
            ulong version = value.GetVersion();
            Task<bool> result = cache.ReplaceWithVersionAsync(K1, V2, version);
            result.Wait(5000);
            value = cache.GetVersioned(K1);
            Assert.AreEqual(V2, value.GetValue());
            Assert.IsTrue(value.GetVersion() != version);
        }

        protected void TestPutSize(IRemoteCache<string, string> cache)
        {
            cache.Put(K1, V1);
            Assert.IsTrue(cache.Size() != 0);
        }

        protected void TestPutKeySet(IRemoteCache<string, string> cache)
        {
            cache.Remove(K1);
            cache.Remove(K2);
            ulong before = cache.Size();
            cache.Put(K1, V1);
            cache.Put(K2, V2);
            ISet<string> keyset = cache.KeySet();
            Assert.AreEqual(before + 2, keyset.Count);
        }

        protected void TestStats(IRemoteCache<string, string> cache)
        {
            ServerStatistics stats = cache.Stats();
            Assert.NotNull(stats);
        }

        protected void TestAddRemoveListener(IRemoteCache<string, string> cache)
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Remove(K1);
                cache.Remove(K2);
                cl.filterFactoryName = "";
                cl.converterFactoryName = "";
                cl.AddListener(listener.CreatedEventAction);
                cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
                cache.Put(K1, V1);
                var remoteEvent = listener.PollCreatedEvent();
                Assert.AreEqual(K1, remoteEvent.GetKey());
            }
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        public void TestRemoteTaskExec(IRemoteCache<string, string> cache)
        {
            string scriptName = "script.js";
            string script = "//mode=local,language=javascript\n "
                            + "cache.put(\"k1\", value);\n"
                            + "cache.get(\"k1\");\n";
            scriptCache.Put(scriptName, script);
            Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
            scriptArgs.Add("value", "v1");
            string ret1 = (string)cache.Execute(scriptName, scriptArgs);
            Assert.AreEqual("v1", ret1);
        }
    }
}
