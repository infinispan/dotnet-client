using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Exceptions;
using NLog;
using Infinispan.DotnetClient;
using Infinispan.DotNetClient.Util.Impl;

namespace Infinispan.DotNetClient.Operations
{


    /// <summary>
    /// 
    /// Abstraction of operations that involves a Key. eg: RemoveOperation
    ///   
    /// Author: sunimalr@gmail.com
    /// </summary>

    public abstract class AbstractKeyOperation<T> : HotRodOperation
    {
        protected readonly byte[] key;
        private static Logger logger;

        protected AbstractKeyOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags) :
            base(codec, flags, cacheName, topologyId)
        {
            this.key = key;
            logger = LogManager.GetLogger("AbstractKeyOperation");
        }

        /// <summary>
        /// Sends a request which involves only a 'key' (no value) to the server
        /// </summary>
        /// <param name="key"></param>
        /// <param name="transport"></param>
        /// <param name="opCode">OPeration Code of the request</param>
        /// <param name="opRespCode">Expected response code</param>
        /// <returns>Status returned form the server</returns>
        protected byte SendKeyOperation(byte[] key, ITransport transport, byte opCode, byte opRespCode)
        {
            // 1) write [header][key length][key]
            HeaderParams param = WriteHeader(transport, opCode);
            transport.writeArray(key);
            transport.getBinaryWriter().Flush(); //TODO: Hide Binary Writer here
            // 2) now read the header
            return ReadHeaderAndValidate(transport, param);
        }

        /// <summary>
        /// USed to get the versioned operation response. It contains version information of the data in the server.  
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <returns>Version data on the entry picked</returns>
        protected VersionedOperationResponse ReturnVersionedOperationResponse(ITransport transport, HeaderParams param)
        {
            byte respStatus = ReadHeaderAndValidate(transport, param);
            if (logger.IsTraceEnabled)
                logger.Trace("Read response status : " + respStatus);
            VersionedOperationResponse.RspCode code;
            if (respStatus == NO_ERROR_STATUS)
            {
                code = VersionedOperationResponse.RspCode.SUCCESS;
            }
            else if (respStatus == NOT_PUT_REMOVED_REPLACED_STATUS)
            {
                code = VersionedOperationResponse.RspCode.MODIFIED_KEY;
            }
            else if (respStatus == KEY_DOES_NOT_EXIST_STATUS)
            {
                code = VersionedOperationResponse.RspCode.NO_SUCH_KEY;
            }
            else
            {
                IllegalStateException e = new IllegalStateException("Unknown response status: " + respStatus);
                logger.Warn(e);
                throw e;
            }
            logger.Trace("Response Status : " + code);
            byte[] prevValue = ReturnPossiblePrevValue(transport);
            return new VersionedOperationResponse(prevValue, code);
        }

        /// <summary>
        /// If forceReturnValue is true, this returns the previous value of the entry.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns>Previous value of the queried entry</returns>
        protected byte[] ReturnPossiblePrevValue(ITransport transport)
        {
            if (HasForceReturn(flags))
            {
                byte[] bytes = transport.readArray();
                if (logger.IsTraceEnabled)
                    logger.Trace("Previous value bytes received");
                //0-length response means null
                if ((bytes == null) || (bytes.Length == 0))
                    return null;
                else
                    return bytes;
            }
            else
            {
                return null;
            }
        }

        private bool HasForceReturn(Flag[] flags)
        {
            if (flags == null)
            {
                return false;
            }

            bool hasForceReturnFlag = false;
            foreach (Flag f in flags)
            {
                if (f.getFlagInt() == Flag.FORCE_RETURN_VALUE)
                    hasForceReturnFlag = true;
            }

            return hasForceReturnFlag;
        }
    }
}
