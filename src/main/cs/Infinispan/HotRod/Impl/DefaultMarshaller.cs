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
    public class DefaultMarshaller : IMarshaller
    {
        private static Logger logger;

        public DefaultMarshaller()
        {
            logger = LogManager.GetLogger("DefaultMarshaller");
        }

        public byte[] ObjectToByteBuffer(Object obj, int estimatedSize) {
            return ObjectToByteBuffer(obj);
        }

        public byte[] ObjectToByteBuffer(Object obj) {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            formatter.Serialize(stream, obj);
            if (logger.IsTraceEnabled) {
                logger.Trace("Serialized : " + obj.ToString());
            }
            // Needed?
            stream.Flush();

            return stream.ToArray();
        }

        public Object ObjectFromByteBuffer(byte[] buf) {
            return ObjectFromByteBuffer(buf, 0, buf.Length);
        }

        public Object ObjectFromByteBuffer(byte[] buf, int offset, int length) {
            if (buf == null) {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(buf, offset, length);

            Object result = (Object) formatter.Deserialize(stream);
            if (logger.IsTraceEnabled) {
                logger.Trace("Deserialized : " + result.ToString());
            }
            return result;
        }

        public bool IsMarshallable(Object o) {
            return true;
        }
    }
#pragma warning restore 1591
}
