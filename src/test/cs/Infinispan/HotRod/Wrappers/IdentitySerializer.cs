using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NLog;

namespace Infinispan.HotRod.Wrappers
{
#pragma warning disable 1591
    public class IdentitySerializer : ISerializer
    {
        private static Logger logger;

        public IdentitySerializer()
        {
            logger = LogManager.GetLogger("Serializer");
        }

        public byte[] Serialize(Object obj)
        {
            if (obj is byte[]) {
                return (byte[]) obj;
            }
            throw new Exception("Expecting a byte[] as input");
        }

        public Object Deserialize(byte[] dataArray)
        {
            return dataArray;
        }
    }
#pragma warning restore 1591
}
