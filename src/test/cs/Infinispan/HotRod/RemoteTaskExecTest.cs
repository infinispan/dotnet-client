using Infinispan.HotRod.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        [Test]
        public void RemoteTaskTest()
        {
            // Register on the server a js routine that takes 3 sec to return a value
            string script_name = "script.js";
            string script = "// mode=local,language=javascript\n "
                                        + "var cache = cacheManager.getCache(\"default\");\n"
                                        + "cache.put(\"argsKey1\", argsKey1);\n"
                                        + "cache.get(\"argsKey1\");\n";

            IRemoteCache<string, string> scriptCache = remoteManager.GetCache<string, string>(PROTOBUF_SCRIPT_CACHE_NAME);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            scriptCache.Put(script_name, script);

            Dictionary<string, string> scriptArgs = new Dictionary<string, string>();
            byte[] bvalue = marshaller.ObjectToByteBuffer("argValue1");
            string svalue = System.Text.Encoding.UTF8.GetString(bvalue);
            scriptArgs.Add("argsKey1", svalue);
            byte[] ret1 = testCache.Execute(script_name, scriptArgs);
            Assert.AreEqual("argValue1", marshaller.ObjectFromByteBuffer(ret1));
        }
    }
}
