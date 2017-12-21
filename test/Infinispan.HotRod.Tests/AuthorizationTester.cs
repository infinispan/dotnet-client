using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinispan.HotRod.Tests
{
    public class AuthorizationTester
    {
        private const string K1 = "k1";
        private const string V1 = "v1";
        private const string K2 = "k2";
        private const string V2 = "v2";
        private const string NON_EXISTENT_KEY = "nonExistentKey";

        public void TestReaderSuccess(IRemoteCache<String, String> hotrodCache)
        {
            TestContainsKey(hotrodCache);
            TestGetNonExistent(hotrodCache);
            TestGetVersioned(hotrodCache);
            TestGetWithMetadata(hotrodCache);
        }

        public void TestReaderPerformsWrites(IRemoteCache<String, String> hotrodCache)
        {
            AssertError(hotrodCache, cache => TestPut(cache));
            AssertError(hotrodCache, cache => TestPutAsync(cache));
            AssertError(hotrodCache, cache => TestRemoveNonExistent(cache));
            AssertError(hotrodCache, cache => TestRemoveAsyncNonExistent(cache));
        }

        public void TestWriterSuccess(IRemoteCache<String, String> hotrodCache)
        {
            TestPut(hotrodCache);
            TestPutAsync(hotrodCache);
            TestRemoveNonExistent(hotrodCache);
            TestRemoveAsyncNonExistent(hotrodCache);
        }

        public void TestWriterPerformsReads(IRemoteCache<String, String> hotrodCache)
        {
            AssertError(hotrodCache, cache => TestContainsKey(cache));
            AssertError(hotrodCache, cache => TestGetNonExistent(cache));
            AssertError(hotrodCache, cache => TestGetVersioned(cache));
            AssertError(hotrodCache, cache => TestGetWithMetadata(cache));
        }

        public void TestWriterPerformsSupervisorOps(IRemoteCache<String, String> hotrodCache, IRemoteCache<String, String> scriptCache, IMarshaller marshaller)
        {
            AssertError(hotrodCache, cache => TestPutClear(cache));
            AssertError(hotrodCache, cache => TestPutContains(cache));
            AssertError(hotrodCache, cache => TestPutGetBulk(cache));
            AssertError(hotrodCache, cache => TestPutGetVersioned(cache));
            AssertError(hotrodCache, cache => TestPutGetWithMetadata(cache));
            AssertError(hotrodCache, cache => TestPutAll(cache));
            AssertError(hotrodCache, cache => TestPutIfAbsent(cache));
            AssertError(hotrodCache, cache => TestPutRemoveContains(cache));
            AssertError(hotrodCache, cache => TestPutRemoveWithVersion(cache));
            AssertError(hotrodCache, cache => TestPutReplaceWithFlag(cache));
            AssertError(hotrodCache, cache => TestPutReplaceWithVersion(cache));
            AssertError(hotrodCache, cache => TestPutSize(cache));
            AssertError(hotrodCache, cache => TestRemoteTaskExec(cache, scriptCache, marshaller));
        }

        public void TestSupervisorSuccess(IRemoteCache<String, String> hotrodCache, IRemoteCache<String, String> scriptCache, IMarshaller marshaller)
        {
            TestCommonSupervisorAdminOps(hotrodCache, scriptCache, marshaller);
        }

        public void TestSupervisorPerformsAdminOps(IRemoteCache<String, String> hotrodCache)
        {
            AssertError(hotrodCache, cache => TestStats(cache));
            AssertError(hotrodCache, cache => TestAddRemoveListener(cache));
        }

        public void TestAdminSuccess(IRemoteCache<String, String> hotrodCache, IRemoteCache<String, String> scriptCache, IMarshaller marshaller)
        {
            TestCommonSupervisorAdminOps(hotrodCache, scriptCache, marshaller);
            TestStats(hotrodCache);
            TestAddRemoveListener(hotrodCache);
            TestPutKeySet(hotrodCache);
        }

        protected void TestCommonSupervisorAdminOps(IRemoteCache<String, String> hotrodCache, IRemoteCache<String, String> scriptCache, IMarshaller marshaller)
        {
            TestPutClear(hotrodCache);
            TestPutClearAsync(hotrodCache);
            TestPutContains(hotrodCache);
            TestPutGet(hotrodCache);
            TestPutGetAsync(hotrodCache);
            TestPutGetBulk(hotrodCache);
            TestPutGetVersioned(hotrodCache);
            TestPutGetWithMetadata(hotrodCache);
            TestPutAll(hotrodCache);
            TestPutAllAsync(hotrodCache);
            TestPutIfAbsent(hotrodCache); //requires both READ and WRITE permissions
            TestPutIfAbsentAsync(hotrodCache);
            TestPutRemoveContains(hotrodCache);
            TestPutRemoveAsyncContains(hotrodCache);
            TestPutRemoveWithVersion(hotrodCache);
            TestPutRemoveWithVersionAsync(hotrodCache);
            TestPutReplaceWithFlag(hotrodCache);
            TestPutReplaceWithVersion(hotrodCache);
            TestPutReplaceWithVersionAsync(hotrodCache);
            TestPutSize(hotrodCache);
            TestRemoteTaskExec(hotrodCache, scriptCache, marshaller);
            //see ISPN-8059 - test this only for Admin user
            //TestPutKeySet(cache);
        }

        protected void AssertError(IRemoteCache<String, String> hotrodCache, Action<IRemoteCache<String, String>> f)
        {
            const string ERROR_MSG = "ERROR: Unauthorized operation performed!";
            try
            {
                f.Invoke(hotrodCache);
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

        public void TestRemoteTaskExec(IRemoteCache<string, string> cache, IRemoteCache<string, string> scriptCache, IMarshaller marshaller)
        {
            string scriptName = "script.js";
            string script = "//mode=local,language=javascript\n "
		            + "var cache = cacheManager.getCache(\"default\");\n "
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

