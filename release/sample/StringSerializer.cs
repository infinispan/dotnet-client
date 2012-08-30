using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient;

namespace InfinispanDotnetClientSample
{
    class StringSerializer : ISerializer
    {
        public object Deserialize(byte[] dataArray)
        {
            try
            {
                return (object)UTF8Encoding.UTF8.GetString(dataArray);
            }
            catch (ArgumentNullException e)
            {
                return (object)"null";
            }

        }

        public byte[] Serialize(object ob)
        {
            return UTF8Encoding.UTF8.GetBytes((string)ob);
        }
    }
}
