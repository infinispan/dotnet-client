using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.HotRod
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class IdentityMarshaller : IMarshaller
    {
        public bool IsMarshallable(object o)
        {
            throw new NotImplementedException();
        }

        public object ObjectFromByteBuffer(byte[] buf)
        {
            return buf;
        }

        public object ObjectFromByteBuffer(byte[] buf, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public byte[] ObjectToByteBuffer(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("Cannot serialize null");
            }
            else if (obj is byte[])
            {
                return (byte[])obj;
            }
            throw new Exception("Expecting a byte[] as input, but it is: " + obj.ToString());
        }

        public byte[] ObjectToByteBuffer(object obj, int estimatedSize)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
