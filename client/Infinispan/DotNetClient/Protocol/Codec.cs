using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Exceptions;
using NLog;
using Infinispan.DotnetClient;
using System.Net;
using Infinispan.DotNetClient.Operations;

namespace Infinispan.DotNetClient.Protocol
{
    /*
     * Hot Rod Codec for vesion 1.1 of Hot Rod Protocol.
     * 
     * Author: sunimalr@gmail.com
     * 
     */

    public class Codec
    {

        private static Logger logger;
        private RemoteCacheManager cacheManager;

        public Codec(RemoteCacheManager rm)
        {
            cacheManager = rm;
            logger = LogManager.GetLogger("Codec");
        }

        public HeaderParams WriteHeader(ITransport trans, HeaderParams param, byte ver)
        {
            trans.WriteByte(HotRodConstants.REQUEST_MAGIC);
            //TODO: Implement a proper way to increment message ID
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            param.MessageId(r.Next(0, 255));

            trans.WriteVLong(param.Messageid); //message id
            trans.WriteByte(HotRodConstants.VERSION_11);//version
            trans.WriteByte(param.OperCode);//opcode
            trans.WriteVInt(param.Cachename.Length); //default cache name. therefore cache name length is 0
            if (param.Cachename.Length != 0)
            {
                trans.WriteArray(param.Cachename); // Cache name is not used for default cache
            }
            int flagInt = 0x00; //0x00 Used here since intention is to use no flags

            if (param.Flag != null)
            {
                foreach (Flag f in param.Flag)
                {
                    flagInt = f.GetFlagInt() | flagInt;
                }
            }

            trans.WriteVInt(flagInt);//flag is 0 for base clients
            trans.WriteByte(param.Clientintel);
            trans.WriteVInt(cacheManager.GetTopologyId());//for basic clients topology ID = 0 
            if (logger.IsTraceEnabled)
                logger.Trace("topologyID Sent = " + param.Topologyid);
            trans.WriteByte(param.Txmarker);
            return param;
        }

        public byte ReadHeader(ITransport trans, HeaderParams param, OperationsFactory opFac, HotRodOperation op)
        {
            byte magic = trans.ReadByte(); //Reads magic byte: indicates whether the header is a request or a response

            if (magic != HotRodConstants.RESPONSE_MAGIC)
            {
                String message = "Invalid magic number! Expected " + HotRodConstants.RESPONSE_MAGIC + "received " + magic;
                InvalidResponseException e = new InvalidResponseException(message);
                logger.Warn(e);
                throw e;
            }

            long receivedMessageId = trans.ReadVLong(); //Reads the message ID. Should be similar to the msg ID of the request
            
            if (receivedMessageId != param.Messageid && receivedMessageId != 0)
            {
                String message = "Invalid Message ID! Expected " + param.Messageid + "received " + receivedMessageId;
                InvalidResponseException e = new InvalidResponseException(message);
                logger.Warn(e);
                throw new InvalidResponseException(message);
            }

            byte receivedOpCode = trans.ReadByte(); //Reads the OP Code

            logger.Trace(String.Format("response code recieved = " + receivedOpCode));
            if (receivedOpCode != param.OpRespCode) //Checks whether the recieved OP code is the corrsponding OPCode for the request sent
            {
                if (receivedOpCode == HotRodConstants.ERROR_RESPONSE) //In case of any error indication by the server
                {
                    logger.Warn(String.Format("Error Response Recieved : " + receivedOpCode));
                    throw new InvalidResponseException("Error Response Recieved");
                }

                logger.Warn(String.Format("Invalid Response Recieved : Expected " + param.OpRespCode + " Recieved " + receivedOpCode));
                throw new InvalidResponseException("Invalid Response Operation.");
            }


            byte status = trans.ReadByte(); //Reads the status
            logger.Trace(String.Format("Status : " + status));

            byte topchange = trans.ReadByte(); //Reads the Topology change byte. Equals 0 for No Change in topology

            logger.Trace(String.Format("Topology change indicator value : " + topchange));

            int newTopology = param.Topologyid;

            if (topchange == 1)
                ReadNewTopologyAndHash(trans, param, opFac, op);

            return status;
        }

        public void ReadNewTopologyAndHash(ITransport transport, HeaderParams param, OperationsFactory opFac, HotRodOperation op)
        {
            int newTopologyId = transport.ReadVInt();
            cacheManager.SetTopologyId(newTopologyId);
            int numOfServers = transport.ReadVInt();

            if (logger.IsTraceEnabled)
            {
                logger.Trace("Topology change request: newTopologyId= " + newTopologyId + " numOfServers= " + numOfServers);
            }
            List<Tuple<string, int>> newServerList = new List<Tuple<string, int>>();
            for (int i = 0; i < numOfServers; i++)
            {
                string host = transport.ReadString(); //int hostIPLength will be read inside the ReadString Method.
                int port = transport.ReadUnsignedShort();
                if (logger.IsTraceEnabled)
                {
                    logger.Trace("Server read : " + host + ":" + port);
                }
                Tuple<string, int> newServer = new Tuple<string, int>(host, port);
                newServerList.Add(newServer);
            }
            transport.GetTransportFactory().UpdateServers(newServerList);
        }
    }
}
