using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class AuthorizationTester
    {
        private const string K1 = "k1";
        private const string V1 = "v1";
        private const string K2 = "k2";
        private const string V2 = "v2";
        private const string NON_EXISTENT_KEY = "nonExistentKey";

        public void TestReaderSuccess(Cache<String, String> hotrodCache)
        {
            TestContainsKey(hotrodCache);
            TestGetNonExistent(hotrodCache);
            TestGetVersioned(hotrodCache);
            TestGetWithMetadata(hotrodCache);
        }

        public async void TestReaderPerformsWrites(Cache<String, String> hotrodCache)
        {
            var excpt = Assert.Throws<InfinispanException>(() => TestPut(hotrodCache));
            Assert.Matches("java.lang.SecurityException: ISPN000287: Unauthorized access:.*lacks 'WRITE' permission", excpt.Message);
            excpt = await Assert.ThrowsAsync<InfinispanException>(() => TestPutAsync(hotrodCache));
            Assert.Matches("java.lang.SecurityException: ISPN000287: Unauthorized access:.*lacks 'WRITE' permission", excpt.Message);
            excpt = await Assert.ThrowsAsync<InfinispanException>(() => TestRemoveAsyncNonExistent(hotrodCache));
            Assert.Matches("java.lang.SecurityException: ISPN000287: Unauthorized access:.*lacks 'WRITE' permission", excpt.Message);
            excpt = Assert.Throws<InfinispanException>(() => TestRemoveNonExistent(hotrodCache));
            Assert.Matches("java.lang.SecurityException: ISPN000287: Unauthorized access:.*lacks 'WRITE' permission", excpt.Message);
        }

        public void TestWriterSuccess(Cache<String, String> hotrodCache)
        {
            TestPut(hotrodCache);
            TestPutAsync(hotrodCache);
            TestRemoveNonExistent(hotrodCache);
            TestRemoveAsyncNonExistent(hotrodCache);
        }

        public void TestWriterPerformsReads(Cache<String, String> hotrodCache)
        {
            Assert.Throws<InfinispanException>(() => TestContainsKey(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestGetNonExistent(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestGetVersioned(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestGetWithMetadata(hotrodCache));
            // AssertError(hotrodCache, cache => TestGetNonExistent(cache));
            // AssertError(hotrodCache, cache => TestGetVersioned(cache));
            // AssertError(hotrodCache, cache => TestGetWithMetadata(cache));
        }

        public void TestWriterPerformsSupervisorOps(Cache<String, String> hotrodCache, Cache<String, String> scriptCache)// , Marshaller marshaller)
        {
            Assert.Throws<InfinispanException>(() => TestPutClear(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestPutContains(hotrodCache));
            // AssertError(hotrodCache, cache => TestPutGetBulk(cache));
            Assert.Throws<InfinispanException>(() => TestPutGetVersioned(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestPutGetWithMetadata(hotrodCache));
            // AssertError(hotrodCache, cache => TestPutAll(cache));
            // AssertError(hotrodCache, cache => TestPutIfAbsent(cache));
            Assert.Throws<InfinispanException>(() => TestPutRemoveContains(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestPutRemoveWithVersion(hotrodCache));
            // AssertError(hotrodCache, cache => TestPutReplaceWithFlag(cache));
            Assert.Throws<InfinispanException>(() => TestPutReplaceWithVersion(hotrodCache));
            Assert.Throws<InfinispanException>(() => TestPutSize(hotrodCache));
            // AssertError(hotrodCache, cache => TestRemoteTaskExec(cache, scriptCache, marshaller));
        }

        public void TestSupervisorSuccess(Cache<String, String> hotrodCache, Cache<String, String> scriptCache)//, IMarshaller marshaller)
        {
            TestCommonSupervisorAdminOps(hotrodCache, scriptCache); // , marshaller);
        }

        public void TestSupervisorPerformsAdminOps(Cache<String, String> hotrodCache)
        {
            Assert.Throws<InfinispanException>(() => TestStats(hotrodCache));
            // TODO: implem listeners AssertError(hotrodCache, cache => TestAddRemoveListener(cache));
        }

        public void TestAdminSuccess(Cache<String, String> hotrodCache, Cache<String, String> scriptCache) //, IMarshaller marshaller)
        {
            TestCommonSupervisorAdminOps(hotrodCache, scriptCache); //, marshaller);
            TestStats(hotrodCache);
            // TestAddRemoveListener(hotrodCache);
            // TestPutKeySet(hotrodCache);
        }
        public void TestReaderAccessStats(Cache<String, String> hotrodCache, Cache<String, String> scriptCache) //, IMarshaller marshaller)
        {
            TestStats(hotrodCache);
            // TestAddRemoveListener(hotrodCache);
            // TestPutKeySet(hotrodCache);
        }


        protected void TestCommonSupervisorAdminOps(Cache<String, String> hotrodCache, Cache<String, String> scriptCache)// , IMarshaller marshaller)
        {
            TestPutClear(hotrodCache);
            // TODO: TestPutClearAsync(hotrodCache);
            TestPutContains(hotrodCache);
            // RPL TestPutGet(hotrodCache);
            TestPutGetAsync(hotrodCache).Wait();
            // TestPutGetBulk(hotrodCache);
            TestPutGetVersioned(hotrodCache);
            TestPutGetWithMetadata(hotrodCache);
            // TestPutAll(hotrodCache);
            // TestPutAllAsync(hotrodCache);
            // TestPutIfAbsent(hotrodCache); //requires both READ and WRITE permissions
            // TestPutIfAbsentAsync(hotrodCache);
            TestPutRemoveContains(hotrodCache);
            // TODO: TestPutRemoveAsyncContains(hotrodCache);
            TestPutRemoveWithVersion(hotrodCache);
            // TODO: TestPutRemoveWithVersionAsync(hotrodCache);
            // TestPutReplaceWithFlag(hotrodCache);
            TestPutReplaceWithVersion(hotrodCache);
            // TODO: TestPutReplaceWithVersionAsync(hotrodCache);
            TestPutSize(hotrodCache);
            // TestRemoteTaskExec(hotrodCache, scriptCache, marshaller);
            //see ISPN-8059 - test this only for Admin user
            //TestPutKeySet(cache);
        }

        protected T executeSync<T>(Task<T> task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ag)
            {
                foreach (Exception ex in ag.InnerExceptions)
                {
                    throw ex;
                }
            }
            return task.Result;
        }

        protected void executeSync(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ag)
            {
                foreach (Exception ex in ag.InnerExceptions)
                {
                    throw ex;
                }
            }
        }

        protected void TestContainsKey(Cache<string, string> cache)
        {
            bool res = executeSync<bool>(cache.ContainsKey(NON_EXISTENT_KEY));
            Assert.False(executeSync<bool>(cache.ContainsKey(NON_EXISTENT_KEY)));
        }
        protected void TestGetNonExistent(Cache<string, string> cache)
        {
            Assert.Null(executeSync<string>(cache.Get(NON_EXISTENT_KEY)));
        }

        protected void TestGetVersioned(Cache<string, string> cache)
        {
            Assert.Null(executeSync<ValueWithVersion<string>>(cache.GetWithVersion(NON_EXISTENT_KEY)));
        }

        protected void TestGetWithMetadata(Cache<string, string> cache)
        {
            Assert.Null(executeSync<ValueWithMetadata<string>>(cache.GetWithMetadata(NON_EXISTENT_KEY)));
        }

        protected void TestPut(Cache<string, string> cache)
        {
            var res = cache.Put(K1, V1);
            try
            {
                res.Wait();
            }
            catch (AggregateException exs)
            {
                foreach (var e in exs.InnerExceptions)
                {
                    throw e;
                }
            }
            Assert.Null(res.Result);
        }

        protected Task<string> TestPutAsync(Cache<string, string> cache)
        {
            return cache.Put(K1, V1);
        }

        protected void TestRemoveNonExistent(Cache<string, string> cache)
        {
            var res = cache.Remove(NON_EXISTENT_KEY);
            try
            {
                res.Wait();
            }
            catch (AggregateException exs)
            {
                foreach (var e in exs.InnerExceptions)
                {
                    throw e;
                }
            }
            Assert.Null(res.Result.PrevValue);
        }

        protected Task<(string, bool)> TestRemoveAsyncNonExistent(Cache<string, string> cache)
        {
            Task<(string, bool)> removeAsync = cache.Remove(NON_EXISTENT_KEY);
            return removeAsync;
        }

        protected void TestPutClear(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            executeSync<string>(cache.Put(K2, V2));
            executeSync(cache.Clear());
            Assert.True(executeSync<bool>(cache.IsEmpty()));
        }

        protected async void TestPutClearAsync(Cache<string, string> cache)
        {
            await cache.Put(K1, V1);
            await cache.Put(K2, V2);
            Task task = cache.Clear();
            await task;
            Assert.True(await cache.IsEmpty());
        }

        protected void TestPutContains(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            Assert.True(executeSync<bool>(cache.ContainsKey(K1)));
        }

        protected async void TestPutGet(Cache<string, string> cache)
        {
            await cache.Put(K1, V1);
            Assert.Equal(V1, await cache.Get(K1));
        }

        protected async Task TestPutGetAsync(Cache<string, string> cache)
        {
            Assert.Null(await cache.Put(K1, V1));
            Assert.Equal(V1, await cache.Get(K1));
        }

        // TODO: implement putgetbulk
        // protected void TestPutGetBulk(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     cache.Remove(K2);
        //     ulong before = cache.Size();
        //     cache.Put(K1, V1);
        //     cache.Put(K2, V2);
        //     Assert.Equal(before + 2, cache.GetBulk().Count);
        // }

        protected void TestPutGetVersioned(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            var gwv = executeSync<ValueWithVersion<string>>(cache.GetWithVersion(K1));
            Assert.Equal(V1, gwv.Value);
            Assert.NotEqual(0, gwv.Version);
        }

        protected void TestPutGetWithMetadata(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            Assert.NotNull(executeSync<ValueWithMetadata<string>>(cache.GetWithMetadata(K1)).Value);
        }

        // TODO: implement putall
        // protected  void TestPutAll(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     cache.Remove(K2);
        //     ulong before = cache.Size();
        //     IDictionary<string, string> entries = new Dictionary<string, string>();
        //     entries.Add(K1, V1);
        //     entries.Add(K2, V2);
        //     cache.PutAll(entries);
        //     Assert.Equal(before + 2, cache.Size());
        // }

        // TODO: implement putall
        //protected void TestPutAllAsync(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     cache.Remove(K2);
        //     ulong before = cache.Size();
        //     IDictionary<string, string> entries = new Dictionary<string, string>();
        //     entries.Add(K1, V1);
        //     entries.Add(K2, V2);
        //     Task result = cache.PutAllAsync(entries);
        //     result.Wait(5000);
        //     Assert.Equal(before + 2, cache.Size());
        // }

        // TODO: implement PutIfAbsent
        // protected void TestPutIfAbsent(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     Assert.Null(cache.PutIfAbsent(K1, V1));
        //     //this should not change the value
        //     cache.PutIfAbsent(K1, V2);
        //     Assert.Equal(V1, cache.Get(K1));
        // }

        // protected void TestPutIfAbsentAsync(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     Task<string> result = cache.PutIfAbsentAsync(K1, V1);
        //     Assert.Null(result.Result);
        //     //this should not change the value
        //     result = cache.PutIfAbsentAsync(K1, V2);
        //     Assert.Equal(V1, cache.Get(K1));
        // }

        protected void TestPutRemoveContains(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            Assert.True(executeSync<bool>(cache.ContainsKey(K1)));
            executeSync<(string, bool)>(cache.Remove(K1));
            Assert.False(executeSync<bool>(cache.ContainsKey(K1)));
        }

        protected async void TestPutRemoveAsyncContains(Cache<string, string> cache)
        {
            await cache.Put(K1, V1);
            Assert.True(await cache.ContainsKey(K1));
            Task<(string, bool)> result = cache.Remove(K1);
            result.Wait(5000);
            Assert.False(await cache.ContainsKey(K1));
        }

        protected void TestPutRemoveWithVersion(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            ValueWithVersion<string> value = executeSync<ValueWithVersion<string>>(cache.GetWithVersion(K1));
            long version = value.Version;
            executeSync<(string, bool)>(cache.RemoveWithVersion(K1, version));
            value = executeSync<ValueWithVersion<string>>(cache.GetWithVersion(K1));
            if (value != null)
            {
                Assert.NotEqual(value.Version, version);
            }
        }

        protected async void TestPutRemoveWithVersionAsync(Cache<string, string> cache)
        {
            await cache.Put(K1, V1);
            ValueWithVersion<string> value = await cache.GetWithVersion(K1);
            long version = value.Version;
            Task<(string, bool)> result = cache.RemoveWithVersion(K1, version);
            result.Wait(5000);
            value = await cache.GetWithVersion(K1);
            if (value != null)
            {
                Assert.Equal(value.Version, version);
            }
        }

        // TODO: implement WithFlag logic
        // protected async void TestPutReplaceWithFlag(Cache<string, string> cache)
        // {
        //     await cache.Put(K1, V1);
        //     Assert.Equal(V1, cache.WithFlags(Flags.FORCE_RETURN_VALUE).Replace(K1, V2));
        //     Assert.Equal(V2, cache.Get(K1));
        // }

        protected void TestPutReplaceWithVersion(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            var value = executeSync<ValueWithVersion<string>>(cache.GetWithVersion(K1));
            long version = value.Version;
            executeSync<bool>(cache.ReplaceWithVersion(K1, V2, version));
            value = executeSync<ValueWithVersion<string>>(cache.GetWithVersion(K1));
            Assert.Equal(V2, value.Value);
            Assert.True(value.Version != version);
        }

        protected async void TestPutReplaceWithVersionAsync(Cache<string, string> cache)
        {
            await cache.Put(K1, V1);
            ValueWithVersion<string> value = await cache.GetWithVersion(K1);
            long version = value.Version;
            Task<bool> result = cache.ReplaceWithVersion(K1, V2, version);
            result.Wait(5000);
            value = await cache.GetWithVersion(K1);
            Assert.Equal(V2, value.Value);
            Assert.True(value.Version != version);
        }

        protected void TestPutSize(Cache<string, string> cache)
        {
            executeSync<string>(cache.Put(K1, V1));
            Assert.NotEqual(0, executeSync<int>(cache.Size()));
        }

        // TODO: implement PutKeySet
        // protected async void TestPutKeySet(Cache<string, string> cache)
        // {
        //     cache.Remove(K1);
        //     cache.Remove(K2);
        //     ulong before = cache.Size();
        //     cache.Put(K1, V1);
        //     cache.Put(K2, V2);
        //     ISet<string> keyset = cache.KeySet();
        //     Assert.Equal(before + 2, keyset.Count);
        // }

        protected void TestStats(Cache<string, string> cache)
        {
            var task = cache.Stats();
            try
            {
                task.Wait();
            }
            catch (AggregateException exs)
            {
                foreach (var e in exs.InnerExceptions)
                {
                    throw e;
                }
            }
            ServerStatistics stats = task.Result;
            Assert.NotNull(stats);
        }

        // TODO: implement listeners
        // protected void TestAddRemoveListener(Cache<string, string> cache)
        // {
        //     LoggingEventListener<string> listener = new LoggingEventListener<string>();
        //     Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
        //     try
        //     {
        //         cache.Remove(K1);
        //         cache.Remove(K2);
        //         cl.filterFactoryName = "";
        //         cl.converterFactoryName = "";
        //         cl.AddListener(listener.CreatedEventAction);
        //         cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
        //         cache.Put(K1, V1);
        //         var remoteEvent = listener.PollCreatedEvent();
        //         Assert.Equal(K1, remoteEvent.GetKey());
        //     }
        //     finally
        //     {
        //         if (cl.listenerId != null)
        //         {
        //             cache.RemoveClientListener(cl);
        //         }
        //     }
        // }

        // TODO: implement execute
        // public void TestRemoteTaskExec(Cache<string, string> cache, Cache<string, string> scriptCache, IMarshaller marshaller)
        // {
        //     string scriptName = "script.js";
        //     string script = "//mode=local,language=javascript\n "
        //             + "var cache = cacheManager.getCache(\"default\");\n "
        //                     + "cache.put(\"k1\", value);\n"
        //                     + "cache.get(\"k1\");\n";
        //     DataFormat df = new DataFormat();
        //     df.KeyMediaType= "application/x-jboss-marshalling";
        //     df.ValueMediaType = "application/x-jboss-marshalling";
        //     Cache<String, String> dfScriptCache = scriptCache.WithDataFormat(df);
        //     dfScriptCache.Put(scriptName, script);
        //     Cache<String, String> dfCache = cache.WithDataFormat(df);
        //     Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
        //     scriptArgs.Add("value", "v1");
        //     string ret1 = (string)dfCache.Execute(scriptName, scriptArgs);
        //     Assert.Equal("v1", ret1);
        // }
    }
}

