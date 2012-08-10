using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;

namespace Infinispan.DotNetClient
{
    public class VersionedValue
    {
        private byte[] value;
        private Serializer serializer;
        private long ver;

        public VersionedValue(long ver, byte[] value)
        {
            this.ver = ver;
            this.value = value;
            this.serializer = new DefaultSerializer();
        }
         

        public Object getValue()
        {
            return serializer.deserialize(this.value);
            
        }

        public void Value(byte[] val)
        {
            this.value = val;
        }


        public long Ver
        {
            get { return ver; }
            set { ver = value; }
        }

        

      

        
    }
}
