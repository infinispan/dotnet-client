using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;


namespace Infinispan.DotNetClient.Util
{
    /// <summary>
    ///This class keeps configuration settings and also reads them from the app.config file 
    ///Author: sunimalr@gmail.com
    /// </summary>
    public class ClientConfig
    {
        private string serverIP;
        private int serverPort;
        private string cacheName;
        private int topologyId;
        private bool forceReturnValue;

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
            this.serverIP = ServerIP;
            this.cacheName = CacheName;
            this.serverPort = ServerPort;
            this.topologyId = TopologyID;
            this.forceReturnValue = ForceReturnValue;
        }

        /// <summary>
        /// This is the default constructer and can be used in a situation where configuration settings mentioned in XML config files are used.
        /// </summary>
        public ClientConfig()
        {
            this.serverIP = readAttr("serverIP");
            this.serverPort = int.Parse(readAttr("serverPort"));
            this.topologyId = int.Parse(readAttr("topologyId"));
            if (readAttr("forceReturnValue").Equals("true"))
            {
                this.forceReturnValue = true;
            }
            else
            {
                this.forceReturnValue = false;
            }
        }

        public string readAttr(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

    }
}
