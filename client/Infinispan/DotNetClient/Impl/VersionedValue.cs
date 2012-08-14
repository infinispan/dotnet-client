using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Util.Impl;

namespace Infinispan.DotNetClient.Impl
{
    public class VersionedValue : IVersionedValue
    {
        private byte[] value;
        private ISerializer serializer;
        private long ver;

        public VersionedValue(long ver, byte[] value, ISerializer serializer)
        {
            this.ver = ver;
            this.value = value;
            this.serializer = serializer;
        }
         
        public Object GetValue()
        {
            return serializer.deserialize(this.value);
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
