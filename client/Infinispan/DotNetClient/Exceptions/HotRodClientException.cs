using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient.Exceptions
{
    public class HotRodClientException : Exception
    {

        private long messageId = -1;
        private int errorStatusCode = -1;

        public HotRodClientException()
        {
        }

        public HotRodClientException(string message) :
            base(message)
        {
        }

        public HotRodClientException(Exception inner) :
            base("",inner)
        {
        }

        public HotRodClientException(string message, Exception inner) :
            base(message, inner)
        {
        }

        public HotRodClientException(string message, long msgId, int errorStatus)
        {
            this.messageId = msgId;
            this.errorStatusCode = errorStatus;
        }

        protected HotRodClientException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
        }


        public string toString()
        {
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append(": ");
            if (messageId != -1) sb.Append("Request for message id[").Append(messageId).Append("]");
            if (errorStatusCode != -1) sb.Append(" returned ").Append(toErrorMsg(errorStatusCode));
            String message = this.Message;
            if (message != null) sb.Append(": ").Append(message);
            return sb.ToString();
        }


        private string toErrorMsg(int errorStatusCode)
        {
            return string.Format("server error (status=0x%x)", errorStatusCode);
        }


    }
}
