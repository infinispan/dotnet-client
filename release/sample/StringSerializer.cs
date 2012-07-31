using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;

namespace InfinispanDotnetClientSample
{
    class StringSerializer : Serializer
    {
        public object deserialize(byte[] dataArray)
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

        public byte[] serialize(object ob)
        {
            return UTF8Encoding.UTF8.GetBytes((string)ob);
        }
    }
}
