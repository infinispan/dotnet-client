using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infinispan.HotRod;
using Infinispan.HotRod.Config;

namespace nativeTestSuite
{
    class RemoteExecTest
    {
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";

        public static void testMain(string[] args)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            IMarshaller m = new JBasicMarshaller();
            conf.Marshaller(m);
            RemoteCacheManager remote = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<String, String> scriptCache = remote.GetCache<String, String>(PROTOBUF_SCRIPT_CACHE_NAME);
            IRemoteCache<String,String> testCache = remote.GetCache<String,String>();

            String script_name = "script.js";
                        String  script= "// mode=local,language=javascript\n "
                                        + "var cache = cacheManager.getCache();\n"
                                        + "cache.put(\"a\", \"abc\");\n"
                                        + "cache.put(\"b\", \"b\");\n"
                                        + "cache.get(\"a\");\n";
                        scriptCache.Put(script_name, script);
            Dictionary<string, string> scriptArgs = new Dictionary<string, string>();
            byte[] bvalue = m.ObjectToByteBuffer("argValue1");
            String svalue = System.Text.Encoding.UTF8.GetString(bvalue);
            scriptArgs.Add("argsKey1", svalue);
            byte[] ret1 = testCache.Execute(script_name, scriptArgs);
            System.Console.WriteLine("Script execution result: " + m.ObjectFromByteBuffer(ret1));
        }
    }
}
