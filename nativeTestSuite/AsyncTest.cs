using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infinispan.HotRod;
using Infinispan.HotRod.Config;

namespace nativeTestSuite
{
    class AsyncTest
    {
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";

        public static void main(string[] args)
        {
            testMain(args);
        }


        public static void testMain(string[] args)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(6000);
            IMarshaller m = new JBasicMarshaller();
            conf.Marshaller(m);
            RemoteCacheManager remote = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<String, String> scriptCache = remote.GetCache<String, String>(PROTOBUF_SCRIPT_CACHE_NAME);
            IRemoteCache<String, String> testCache = remote.GetCache<String, String>();
            // Register on the server a js routine that takes 3 sec to return a value
            String script_name = "script.js";
            String script = "// mode=local,language=javascript\n "
                            + "var cache = cacheManager.getCache(); "
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
            scriptCache.Put(script_name, script);
            Dictionary<string, string> scriptArgs = new Dictionary<string, string>();

            Task<String> futureExec = Task.Factory.StartNew<String> (() => (String)m.ObjectFromByteBuffer(testCache.Execute(script_name, scriptArgs)));

            // Do something in the meanwhile

            testCache.Put("syncKey", "syncVal");
            String val = testCache.Get("syncKey");
            System.Console.WriteLine("Get value for syncKey: " + val);

            // Get the async result
            System.Console.WriteLine("Script execution result: " + futureExec.Result);
        }

    }
}
