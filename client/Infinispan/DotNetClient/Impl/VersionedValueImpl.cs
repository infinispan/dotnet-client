using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;

namespace Infinispan.DotNetClient.Impl
{
    public class VersionedValueImpl : IVersionedValue
    {
        private byte[] value;
        private ISerializer serializer;
        private long ver;

        public VersionedValueImpl(long ver, byte[] value, ISerializer serializer)
        {
            this.ver = ver;
            this.value = value;
            this.serializer = serializer;
        }
         
        public Object GetValue()
        {
            return serializer.Deserialize(this.value);
        }

        public void SetValue(byte[] val)
        {
            this.value = val;
        }

        public long GetVersion()
        {
            return this.ver;
        }
    }
}
