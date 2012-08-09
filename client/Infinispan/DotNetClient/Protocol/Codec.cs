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

        public Codec()
        {
            logger = LogManager.GetLogger("Codec");
        }

        public HeaderParams writeHeader(ITransport trans, HeaderParams param, byte ver)
        {

            trans.writeByte(HotRodConstants.REQUEST_MAGIC);


            //TODO: Implement a proper way to increment message ID

            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            
            param.messageId(r.Next(0, 255));
            
            trans.writeVLong(param.Messageid); //message id
            trans.writeByte(HotRodConstants.VERSION_11);//version
            trans.writeByte(param.OperCode);//opcode
            trans.writeVInt(param.Cachename.Length); //default cache name. therefore cache name length is 0
            if (param.Cachename.Length != 0)
            {
                trans.writeArray(param.Cachename); // Cache name is not used for default cache
            }
            int flagInt = 0x00; //0x00 Used here since intention is to use no flags

            
            if(param.Flag!=null)
            {
                foreach(Flag f in param.Flag)
                {
                    flagInt = f.getFlagInt() | flagInt;
                }
            }
             
            trans.writeVInt(flagInt);//flag is 0 for base clients
            trans.writeByte(param.Clientintel);
            trans.writeVInt(param.Topologyid);//for basic clients topology ID = 0 
            trans.writeByte(param.Txmarker);
           

            return param;

        }



        public byte readHeader(ITransport trans, HeaderParams param)
        {


            byte magic = trans.readByte(); //Reads magic byte: indicates whether the header is a request or a response

            if (magic != HotRodConstants.RESPONSE_MAGIC)
            {
                String message = "Invalid magic number! Expected "+HotRodConstants.RESPONSE_MAGIC+ "received " + magic;
                InvalidResponseException e = new InvalidResponseException(message);
                logger.Warn(e);
                throw e;
            }

            long receivedMessageId = trans.readVLong(); //Reads the message ID. Should be similar to the msg ID of the request


            if (receivedMessageId != param.Messageid && receivedMessageId != 0)
            {
                String message = "Invalid Message ID! Expected " + param.Messageid + "received " + receivedMessageId;
                InvalidResponseException e = new InvalidResponseException(message);
                logger.Warn(e);
                throw new InvalidResponseException(message);
            }

            byte receivedOpCode = trans.readByte(); //Reads the OP Code

            logger.Trace(String.Format("response code recieved = " + receivedOpCode));
            if (receivedOpCode != param.OpRespCode) //Checks whether the recieved OP code is the corrsponding OPCode for the request sent
            {
                if (receivedOpCode == HotRodConstants.ERROR_RESPONSE) //In case of any error indication by the server
                {
                    
                    logger.Warn(String.Format("Error Response Recieved : "+receivedOpCode));
                    throw new  InvalidResponseException("Error Response Recieved");
                }
                
                logger.Warn(String.Format("Invalid Response Recieved : Expected " + param.OpRespCode+" Recieved " +receivedOpCode));
                throw new InvalidResponseException("Invalid Response Operation.");

            }

            
            byte status = trans.readByte(); //Reads the status
            logger.Trace(String.Format("Status : " + status));

            byte topchange = trans.readByte(); //Reads the Topology change byte. Equals 0 for No Change in topology

            logger.Trace(String.Format("Topology change indicator value : "+topchange));
          
             if (topchange == 1)
         readNewTopologyAndHash(trans, param.Topologyid);

            return status;
        }

        public void readNewTopologyAndHash(ITransport transport, int topologyId) {
      
      int newTopologyId = transport.readVInt();
      topologyId=newTopologyId;
      int numKeyOwners = transport.readUnsignedShort();
      short hashFunctionVersion = transport.readByte();
      int hashSpace = transport.readVInt();
      int clusterSize = transport.readVInt();

            if(logger.IsTraceEnabled)
            {
                logger.Trace("Topology change request: newTopologyId= "+newTopologyId+" numKeyOwners= "+numKeyOwners+ " hashFunctionVersion= "+hashFunctionVersion+" hashSpaceSize= "+hashSpace+" clusterSize= "+clusterSize);
            }
      Dictionary<IPEndPoint, HashSet<int>> servers2Hash = new Dictionary<IPEndPoint, HashSet<int>>();

      for (int i = 0; i < clusterSize; i++) {
         String host = transport.readString();
         int port = transport.readUnsignedShort();
         int hashCode = transport.read4ByteInt();
         if (logger.IsTraceEnabled)
         {
             logger.Trace("Server read : "+host+":"+port+" - hash code is "+hashCode);
         }
         IPEndPoint address = new IPEndPoint(IPAddress.Parse(host), port);
         HashSet<int> hashes;
         servers2Hash.TryGetValue(address,out hashes);
         if (hashes == null) {
            hashes = new HashSet<int>();
            servers2Hash.Add(address,hashes);
         }
         hashes.Add(hashCode);
         if (logger.IsTraceEnabled)
         {
             logger.Trace("Hash code is: "+ hashCode);
         }
         
      }
                  
      transport .getTransportFactory().updateServers(servers2Hash);
      if (hashFunctionVersion == 0) {
         logger.Trace("Not using a consistent hash function");
      } else {
         transport.getTransportFactory().updateHashFunction(servers2Hash, numKeyOwners, hashFunctionVersion, hashSpace);
      }
            
   }


    }
}
