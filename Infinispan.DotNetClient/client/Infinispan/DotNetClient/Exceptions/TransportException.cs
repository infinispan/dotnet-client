using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient.Exceptions
{
    public class TransportException : HotRodClientException
    {

        public TransportException()
        {
        }

        public TransportException(string message):
            base(message)
        {
        }

        public TransportException(string message, Exception cause):
            base(message,cause)
        {
        }

        public TransportException(Exception cause):
            base(cause)
        {
        }
    }
}
