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
    /// <summary>
    /// Provides an adapter from the deprecated ISerializer to IMarsharller.
    /// </summary>
    public class SerializerAdapter : IMarshaller
    {
        private ISerializer serializer;

        public SerializerAdapter(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public byte[] ObjectToByteBuffer(Object obj, int estimatedSize) {
            return ObjectToByteBuffer(obj);
        }

        public byte[] ObjectToByteBuffer(Object obj) {
            return serializer.Serialize(obj);
        }

        public Object ObjectFromByteBuffer(byte[] buf) {
            return serializer.Deserialize(buf);
        }

        public Object ObjectFromByteBuffer(byte[] buf, int offset, int length) {
             throw new NotSupportedException("Partial buf unmarshalling not supportedfor serializer adapters.");
        }

        public bool IsMarshallable(Object o) {
            return true;
        }
    }
#pragma warning restore 1591
}
