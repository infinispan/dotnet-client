using Infinispan.HotRod.Config;
using System.Collections.Generic;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    class RemoteTaskExecTest
    {
        RemoteCacheManager remoteManager;
        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        IMarshaller marshaller;

        [TestFixtureSetUp]
        public void BeforeClass()
        {
        }

        [Test]
        public void RemoteTaskWithArgs()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            string script = "// mode=local,language=javascript \n"
            + "cache.put(k, v);\n"
            + "cache.get(k);\n";
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME, new JBasicMarshaller());
            const string scriptName = "put-get.js";
            scriptCache.Put(scriptName, script);
            Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
            scriptArgs.Add("k", "mykey");
            scriptArgs.Add("v", "myvalue");
            object v = testCache.Execute(scriptName, scriptArgs);
            Assert.AreEqual("myvalue", v);
        }

        [Test]
        public void RemoteTaskWithIntArgs()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            string script = "// mode=local,language=javascript \n"
            + "var x = k+v; \n"
            + "x; \n";
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME, new JBasicMarshaller());
            const string scriptName = "putit-get.js";
            scriptCache.Put(scriptName, script);
            Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
            scriptArgs.Add("k", (int)1);
            scriptArgs.Add("v", (int)2);
            object v = testCache.Execute(scriptName, scriptArgs);
            Assert.AreEqual(3, v);
        }

        [Test]
        public void RemoteTaskJBasicMarshallerTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.ProtocolVersion("2.6");
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            RemoteTaskTest();
        }

        [Test]
        public void RemoteTaskDefaultMarshallerTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            remoteManager = new RemoteCacheManager(conf.Build(),  true);
            RemoteTaskTest();
        }

        public void RemoteTaskTest()
        {
            // Register on the server a js routine that takes 3 sec to return a value
            string scriptName = "script.js";
            string script = "// mode=local,language=javascript\n "
                                        + "var cache = cacheManager.getCache(\"default\");\n"
                                        + "cache.put(\"argsKey1\", argsKey1);\n"
                                        + "cache.get(\"argsKey1\");\n";
            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME, new JBasicMarshaller());
            IRemoteCache<string, long> testCache = remoteManager.GetCache<string, long>();
            try
            {
                scriptCache.Put(scriptName, script);
                Dictionary<string, object> scriptArgs = new Dictionary<string, object>();
                scriptArgs.Add("argsKey1", "argValue1");
                string ret1 = (string)testCache.Execute(scriptName, scriptArgs);
                Assert.AreEqual("argValue1", ret1);
            }
            finally
            {
                testCache.Clear();
            }
        }

        [Test]
        public void RemoteTaskEmptyArgsJBasicMarshallerTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            RemoteTaskEmptyArgsTest();
        }

        [Test]
        public void RemoteTaskEmptyArgsDefaultMarshallerTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            RemoteTaskEmptyArgsTest();
        }

        public void RemoteTaskEmptyArgsTest()
        {
            // Register on the server a js routine that takes 3 sec to return a value
            string scriptName = "script.js";
            string script = "// mode=local,language=javascript\n "
                                        + "var cache = cacheManager.getCache(\"default\");\n"
                                        + "cache.put(\"argsKey1\", \"argValue1\");\n"
                                        + "cache.get(\"argsKey1\");\n";
            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME, new JBasicMarshaller());
            IRemoteCache<string, long> testCache = remoteManager.GetCache<string, long>();
            try
            {
                scriptCache.Put(scriptName, script);
                string ret1 = (string)testCache.Execute(scriptName);
                Assert.AreEqual("argValue1", ret1);
            }
            finally
            {
                testCache.Clear();
            }
        }

        [Test]
        public void RemoteMapReduceWithStreamsTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            try
            {
                const string scriptName = "wordCountStream.js";
                ScriptUtils.LoadTestCache(testCache, "macbeth.txt");
                ScriptUtils.LoadScriptCache(scriptCache, scriptName, scriptName);

                Dictionary<string, object> scriptArgs = new Dictionary<string, object>();

                var result = (System.Int64)testCache.Execute(scriptName, scriptArgs);
                const int expectedMacbethCount = 287;
                Assert.AreEqual(expectedMacbethCount, result);
            }
            finally
            {
                testCache.Clear();
            }
        }
    }
}
