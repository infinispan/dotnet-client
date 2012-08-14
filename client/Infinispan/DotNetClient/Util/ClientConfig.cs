using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using NLog;
using System.Net;

namespace Infinispan.DotNetClient.Util
{
    public class ClientConfig
    {
        private string serverIP;
        private int serverPort;
        private string cacheName;
        private int topologyId;
        private bool forceReturnValue;
        private string serverList;
        private static Logger logger;

        public bool ForceReturnValue
        {
            get { return forceReturnValue; }
            set { forceReturnValue = value; }
        }

        public int TopologyId
        {
            get { return topologyId; }
            set { topologyId = value; }
        }

        public string CacheName
        {
            get { return cacheName; }
            set { cacheName = value; }
        }

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        /// <summary>
        /// This method can be used to override the settings mentioned in the Configuration file.
        /// </summary>
        /// <param name="ServerIP"></param>
        /// <param name="ServerPort"></param>
        /// <param name="CacheName"></param>
        /// <param name="TopologyID"></param>
        /// <param name="ForceReturnValue">If this parameter is true the server sends the previous value which existed before manipulation.</param>
        public ClientConfig(string ServerIP, int ServerPort, string CacheName, int TopologyID, bool ForceReturnValue)
        {
            logger = LogManager.GetLogger("ClientConfig");
            this.serverIP = ServerIP;
            this.cacheName = CacheName;
            this.serverPort = ServerPort;
            this.topologyId = TopologyID;
            this.forceReturnValue = ForceReturnValue;
            this.serverList = "127.0.0.1:11222;";
        }

        
        public ClientConfig(string ServerIP, int ServerPort, string CacheName, int TopologyID, bool ForceReturnValue, string serverlist)
        {
            logger = LogManager.GetLogger("ClientConfig");
            this.serverIP = ServerIP;
            this.cacheName = CacheName;
            this.serverPort = ServerPort;
            this.topologyId = TopologyID;
            this.forceReturnValue = ForceReturnValue;
            this.serverList = serverlist;
        }

        /// <summary>
        /// This is the default constructer and can be used in a situation where configuration settings mentioned in XML config files are used.
        /// </summary>
        public ClientConfig()
        {
            logger = LogManager.GetLogger("ClientConfig");
            this.serverIP = ReadAttr("serverIP");
            this.serverPort = int.Parse(ReadAttr("serverPort"));
            //this.topologyId = int.Parse(readAttr("topologyId"));
            this.serverList = ReadAttr("serverList");
            if (ReadAttr("forceReturnValue").Equals("true"))
            {
                this.forceReturnValue = true;
            }
            else
            {
                this.forceReturnValue = false;
            }
            this.serverList = "127.0.0.1:11222;";
            this.cacheName = "default";
            this.topologyId = 0;
        }

        public string ReadAttr(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        public List<IPEndPoint> GetServerList()
        {
            List<IPEndPoint> tempList = new List<IPEndPoint>();
            string[] splittedlist=serverList.Split(';');
            foreach (string str in splittedlist)
            {
                if (str.Length > 0)
                {
                    logger.Trace("serverlist : " + str);
                    //IPAddress ip = IPAddress.Loopback;
                    IPAddress ip = IPAddress.Parse(str.Split(':')[0]);
                    int port = int.Parse(str.Split(':')[1]);
                    IPEndPoint ep = new IPEndPoint(ip, port);
                    tempList.Add(ep);
                }
            }
            if (tempList.Count > 0)
            {
                return tempList;
            }
            else
            {
                throw new Exception("No initial servers specified!!");
            }
        }
    }
}
