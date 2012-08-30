using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;

namespace Infinispan.DotNetClient.Operations
{ 
    /*
     * Check wheter a gievn key is existing
     *  
     * Author: sunimalr@gmail.com
     * 
     */
    public class ContainsKeyOperation : AbstractKeyOperation<bool>
    {
        public ContainsKeyOperation(Codec codec, byte[] key, byte[] cacheName, Flag[] flags) :
            base(codec, key, cacheName, flags)
        {
        }

        public bool ExecuteOperation(ITransport transport)
        {
            bool containsKey = false;
            short status = SendKeyOperation(key, transport, CONTAINS_KEY_REQUEST, CONTAINS_KEY_RESPONSE);
            if (status == KEY_DOES_NOT_EXIST_STATUS)
            {
                containsKey = false;
            }
            else if (status == NO_ERROR_STATUS)
            {
                containsKey = true;
            }
            return containsKey;
        }
    }
}
