using Infinispan.HotRod.Config;
using Infinispan.HotRod.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    class AsyncOperationsTest
    {
        RemoteCacheManager remoteManager;

        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        IMarshaller marshaller;

        private void InitializeRemoteCacheManager(bool started)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), started);
        }


        [Test]
        public void AsyncRemoteTaskExecTest()
        {
            InitializeRemoteCacheManager(true);
            // Register on the server a js routine that takes 3 sec to return a value
            string script_name = "script.js";
            string script = "// mode=local,language=javascript\n "
                            + "var cache = cacheManager.getCache(\"default\"); "
                            + "cache.put(\"a\", \"abc\"); "
                            + "cache.put(\"b\", \"b\"); "
                            + "var start = (new Date().getTime()); "
                            + "// Now just wait 3 seconds\n"
                            + "for (var i = 0; i < 1000000000; i++) "
                            + "{ "
                            + "  if ((new Date().getTime()) > start + 3000) "
                            + "  { "
                            + "    break; "
                            + "  }; "
                            + "}; "
                            + "cache.get(\"a\");";

            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("default");
            scriptCache.Put(script_name, script);

            Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
            Task<string> futureExec = Task.Factory.StartNew<string>(() => (string)testCache.Execute(script_name, scriptArgs));

            // Do something in the meanwhile
            testCache.Put("syncKey", "syncVal");
            Assert.AreEqual("syncVal", testCache.Get("syncKey"));
            // Get the async result
            Assert.AreEqual("abc", futureExec.Result);
        }

        [Test]
        public void PutAsyncTest()
        {
            InitializeRemoteCacheManager(true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("default");
            Task<string> result = testCache.PutAsync("kasync", "vasync");
            Assert.AreEqual(null, result.Result);
            Assert.IsTrue(result.IsCompleted);
            Assert.Null(result.Exception);
            Assert.AreEqual("vasync", testCache.Get("kasync"));
        }

        [Test]
        [ExpectedException(typeof(RemoteCacheManagerNotStartedException))]
        public void PutAsyncExceptionTest()
        {
            InitializeRemoteCacheManager(false);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("default");
            testCache.PutAsync("kasync", "vasync");
            Assert.Fail("Should not get here");
        }
    }
}
