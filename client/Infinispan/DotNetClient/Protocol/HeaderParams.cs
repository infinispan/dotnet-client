using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Exceptions;
using Infinispan.DotNetClient;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Protocol
{

    /*
    * 
    * This class defines Header Parameters used in hotrod protocol header as defined in 
    * https://docs.jboss.org/author/display/ISPN/Hot+Rod+Protocol+-+Version+1.1
    * 
    * Author: sunimalr@gmail.com
    *      
    */


    public class HeaderParams : HotRodConstants
    {
        private byte operCode;
        private byte opRespCode;
        private byte[] cachename;
        private Flag[] flag;
        private byte clientintel;
        private byte txmarker;
        private int topologyid;
        private long messageid;

        public byte OperCode
        {
            get { return operCode; }
            set { operCode = value; }
        }


        public byte OpRespCode
        {
            get { return opRespCode; }
            set { opRespCode = value; }
        }


        public byte[] Cachename
        {
            get { return cachename; }
            set { cachename = value; }
        }



        internal Flag[] Flag
        {
            get { return flag; }
            set { flag = value; }
        }


        public byte Clientintel
        {
            get { return clientintel; }
            set { clientintel = value; }
        }


        public byte Txmarker
        {
            get { return txmarker; }
            set { txmarker = value; }
        }


        public int Topologyid
        {
            get { return topologyid; }
            set { topologyid = value; }
        }


        public long Messageid
        {
            get { return messageid; }
            set { messageid = value; }
        }

        public HeaderParams OpCode(byte opCode)
        {
            this.operCode = opCode;
            this.opRespCode = ToOpRespCode(opCode);
            return this;
        }

        public HeaderParams CacheName(byte[] name)
        {
            this.cachename = name;
            return this;
        }

        public HeaderParams flags(Flag[] flag)
        {
            this.flag = flag;
            return this;
        }

        public HeaderParams ClientIntel(byte clientintel)
        {
            this.clientintel = clientintel;
            return this;
        }

        public HeaderParams TxMarker(byte txmarker)
        {
            this.txmarker = txmarker;
            return this;
        }

        public HeaderParams TopologyId(int topologyid)
        {
            this.topologyid = topologyid;
            return this;
        }

        public HeaderParams MessageId(long messageid)
        {
            this.messageid = messageid;
            return this;
        }

        private byte ToOpRespCode(byte opCode)
        {
            switch (opCode)
            {
                case HotRodConstants.PUT_REQUEST:
                    return HotRodConstants.PUT_RESPONSE;
                case HotRodConstants.GET_REQUEST:
                    return HotRodConstants.GET_RESPONSE;
                case HotRodConstants.PUT_IF_ABSENT_REQUEST:
                    return HotRodConstants.PUT_IF_ABSENT_RESPONSE;
                case HotRodConstants.REPLACE_REQUEST:
                    return HotRodConstants.REPLACE_RESPONSE;
                case HotRodConstants.REPLACE_IF_UNMODIFIED_REQUEST:
                    return HotRodConstants.REPLACE_IF_UNMODIFIED_RESPONSE;
                case HotRodConstants.REMOVE_REQUEST:
                    return HotRodConstants.REMOVE_RESPONSE;
                case HotRodConstants.REMOVE_IF_UNMODIFIED_REQUEST:
                    return HotRodConstants.REMOVE_IF_UNMODIFIED_RESPONSE;
                case HotRodConstants.CONTAINS_KEY_REQUEST:
                    return HotRodConstants.CONTAINS_KEY_RESPONSE;
                case HotRodConstants.GET_WITH_VERSION:
                    return HotRodConstants.GET_WITH_VERSION_RESPONSE;
                case HotRodConstants.CLEAR_REQUEST:
                    return HotRodConstants.CLEAR_RESPONSE;
                case HotRodConstants.STATS_REQUEST:
                    return HotRodConstants.STATS_RESPONSE;
                case HotRodConstants.PING_REQUEST:
                    return HotRodConstants.PING_RESPONSE;
                case HotRodConstants.BULK_GET_REQUEST:
                    return HotRodConstants.BULK_GET_RESPONSE;
                default:
                    throw new IllegalStateException("Unknown operation code: " + opCode);
            }
        }

    }
}
