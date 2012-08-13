using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Exceptions;
using NLog;
namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    /*
     * Handles all the communication between the server and client at the Transport layer level.
     * 
     * 
     * Author: sunimalr@gmail.com
     * 
     */
    public class TCPTransport : AbstractTransport
    {
        private TcpClient tcpClient;
        private IPAddress serverIP; //IP address of Server
        private int serverPort; //Port of the Server which is used for client server communication.
        private BufferedStream buff; //having a buffered stream prevents sending data for each moment that the client writes an object to the stream. 
        private BinaryWriter bWriter;//BinaryWriter and BinaryReader is used for passing and retrieving data to and from the Underlying Buffered TCP connection stream
        private BinaryReader bReader;
        private static Logger logger;

        public TCPTransport(IPAddress server, int serverPort)
        {
            this.serverIP = server;
            this.serverPort = serverPort;
            logger = LogManager.GetLogger("TCPTransport");
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(server, serverPort);
                this.buff = new BufferedStream(tcpClient.GetStream());
                this.bWriter = new BinaryWriter(buff);
                this.bReader = new BinaryReader(tcpClient.GetStream());
                String message = string.Format("Connected to server :" + server.ToString() + " : " + serverPort);
                logger.Trace(message);
            }
            catch (Exception e)
            {
                String message = string.Format("Could not connect to server :" + server.ToString() + " : " + serverPort);
                logger.Warn("Infinispan.DotNetClient", message);
                throw new TransportException(message, e);
            }
        }

        public IPAddress getIpAddress()
        {
            return ServerIP;
        }

        public int getServerPort()
        {
            return ServerPort;
        }

        public BinaryWriter BWriter
        {
            get { return bWriter; }
            set { bWriter = value; }
        }

        public BinaryReader BReader
        {
            get { return bReader; }
            set { bReader = value; }
        }

public NetworkStream getNetworkOutStream()
        {
            return this.tcpClient.GetStream();
        }

        public IPAddress ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        public override BinaryWriter getBinaryWriter()
        {
            return this.BWriter;
        }

        public override BinaryReader getBinaryReader()
        {
            return this.BReader;
        }

        //Writing and reading Variable Length numerics are handed by UnsignedNumeric Class. 
        //Writes variable length integer varibale to the stream
        public override void writeVInt(int vInt)
        {
            try
            {
                UnsignedNumeric.writeUnsignedInt(this, vInt);

            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
        }

        //Writes variable length long varibale to the stream
        public override void writeVLong(long l)
        {
            try
            {
                UnsignedNumeric.writeUnsignedLong(this, l);
            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
        }

        //Reads variable length long varibale from the stream
        public override long readVLong()
        {
            try
            {

                return UnsignedNumeric.readUnsignedLong(this);
            }
            catch (IOException e)
            {
                logger.Warn(e);
throw new TransportException(e);
            }
        }

        //Reads variable length long varibale to the stream
        public override int readVInt()
        {
            try
            {
                return UnsignedNumeric.readUnsignedInt(this);
            }
            catch (IOException e)
            {
                logger.Warn(e);
throw new TransportException(e);
            }
        }

        //Writes a byte array to the stream
        protected override void writeBytes(byte[] toAppend)
        {
            try
            {
                this.bWriter.Write(toAppend);
            }
            catch (IOException e)
            {
                logger.Warn(e);

                throw new TransportException(e);
            }
        }

        //Writes a single byte to the stream
        public override void writeByte(byte toWrite)
        {
            try
            {
                BufferedStream b = new BufferedStream(tcpClient.GetStream());
                this.bWriter.Write(toWrite);
                logger.Trace(String.Format("Byte Written : " + toWrite));
            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
        }

        //Reads a single byte from the stream
        public override byte readByte()
        {
            int resultInt;
            try
            {
                resultInt = bReader.ReadByte();
                logger.Trace(String.Format("Byte read : " + resultInt));
            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
            if (resultInt == -1)
            {
                logger.Warn("End of stream Reached");
                throw new TransportException("End of stream reached ");
            }
            return (byte)resultInt;
        }

        //Reads a byte array for size "size" from the stream
        public override byte[] readByteArray(int size)
        {
            byte[] result = new byte[size];
            bool done = false;
            int offset = 0;
            do
            {
                int read;
                try
                {
                    int len = size - offset;
                    read = bReader.Read(result, offset, len);
                    logger.Trace(String.Format("Byte array read : " + read));
                }
                catch (IOException e)
                {
                    logger.Warn(e);
                    throw new TransportException(e);
                }
                if (read == -1)
                {
                    logger.Warn("End of stream reached!");
                    throw new TransportException("End of stream reached!");
                }
                if (read + offset == size)
                {
                    done = true;
                }
                else
                {
                    offset += read;
                    if (offset > result.Length)
                    {
                        IllegalStateException e = new IllegalStateException("Assertion");
                        logger.Warn(e);
                        throw new TransportException(e);
                    }
                }
            } while (!done);
            return result;
        }

        //Flushes the Binary Writer
        public override void flush()
        {
            try
            {
                this.bWriter.Flush();
                logger.Trace("Binary Writer Flushed!!");
            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
        }

        //Closes the TCP Client and halts connection with the server
        public override void release()
        {
            try
            {
                tcpClient.Close();
                logger.Trace("TCPClient Closed!!!");
            }
            catch (IOException e)
            {
                logger.Warn(e);
                throw new TransportException(e);
            }
        }

        //cloases the current connection with the server
        public void closeConnection()
        {
            tcpClient.Close();
        }
    }
}