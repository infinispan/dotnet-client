using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;


namespace Infinispan.DotNetClient.Operations
{
    /*
    * Retrieves values from a cache
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class GetOperation : AbstractKeyOperation<byte[]>
    {
        public GetOperation(Codec codec, byte[] key, byte[] cacheName, Flag[] flags) :
            base(codec, key, cacheName, flags)
        {
        }

        public byte[] ExecuteOperation(ITransport transport)
        {
            byte[] result = null;
            byte status = SendKeyOperation(key, transport, HotRodConstants.GET_REQUEST, HotRodConstants.GET_RESPONSE);
            if (status == HotRodConstants.KEY_DOES_NOT_EXIST_STATUS)
            {
                   result = null;
            }
            else
            {
                if (status == HotRodConstants.NO_ERROR_STATUS)
                {
                    result = transport.ReadArray();
                }
            }
            return result;
        }
    }
}
