using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NLog;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    public class DefaultSerializer : ISerializer
    {
        private static Logger logger;

        public DefaultSerializer()
        {
            logger = LogManager.GetLogger("Serializer");
        }

        public byte[] Serialize(Object ob)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            formatter.Serialize(stream, ob);
            if (logger.IsTraceEnabled) {
                logger.Trace("Serialized : " + ob.ToString());
            }
            // Needed?
            stream.Flush();

            return stream.ToArray();
        }

        public Object Deserialize(byte[] dataArray)
        {
            if (dataArray == null) {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(dataArray);
            
            Object result = (Object) formatter.Deserialize(stream);
            if (logger.IsTraceEnabled) {
                logger.Trace("Deserialized : " + result.ToString());
            }
            return result;
        }
    }
#pragma warning restore 1591
}
