using System;
using Infinispan.HotRod;

namespace Infinispan.HotRod.Tests.Util
{
    class ScriptUtils
    {
        public static void LoadScriptCache(IRemoteCache<String, String> cache, String scriptKey, String fileName)
        {
            string text = System.IO.File.ReadAllText(fileName);
            cache.Put(scriptKey, text);
        }

        public static void LoadTestCache(IRemoteCache<String, String> cache, String fileName)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            try
            {
                String line;
                int counter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    cache.Put("line" + counter++, line);
                }
            }
            finally
            {
                file.Close();
            }
        }
    }
}
