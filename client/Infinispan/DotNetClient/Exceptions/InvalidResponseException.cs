using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient.Exceptions
{
    public class InvalidResponseException : HotRodClientException
    {
        public InvalidResponseException()
        {
        }

        public InvalidResponseException(string message):
            base(message)
        {
        }

        public InvalidResponseException(string message, Exception cause):
            base(message,cause)
        {
        }

        public InvalidResponseException(Exception cause):
            base(cause)
        {
        }
    }
}
