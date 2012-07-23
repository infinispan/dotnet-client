using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    public class BinaryVersionedValue
    {
        private byte[] value;

        private long ver;

        public BinaryVersionedValue(long ver, byte[] value)
        {
            this.ver = ver;
            this.value = value;
        }


        public byte[] Value
        {
            get { return this.value; }
            set { this.value = value; }
        }


        public long Ver1
        {
            get { return ver; }
            set { ver = value; }
        }

        

      

        
    }
}
