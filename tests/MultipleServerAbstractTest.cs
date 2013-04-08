using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient;
using System.Threading;
using Infinispan.DotNetClient.Impl;

namespace tests
{
    [TestClass]
    public class MultipleServerAbstractTest
    {
        protected Process hrServer1;
        protected Process hrServer2;
        protected ClientConfig conf1 = new ClientConfig("127.0.0.1", 11222, false);
        protected ISerializer serializer1 = new DefaultSerializer();
        protected RemoteCacheManager remoteManager1;
        
        [TestInitialize()]
        public void StartHotrodServer()
        {
            //TODO - we might want to make this an actual process at some point so that the window is no longer displayed
            // http://stackoverflow.com/questions/1113000/how-do-start-stop-services-using-net-stop-command-in-c-sharp
            string ispnHome = System.Environment.GetEnvironmentVariable("ISPN_HOME");
            if (ispnHome == null)
                throw new Exception("you must set the ISPN_HOME variable pointing to the ISPN installation in order to be able to run the tests");

            hrServer1 = new Process();
            hrServer2 = new Process();
            String nameOfBatchFile1 = ispnHome + "\\bin\\startServer1.bat";
            String nameOfBatchFile2 = ispnHome + "\\bin\\startServer2.bat";
            string parameters1 = String.Format("/k \"{0}\"" + " -r hotrod -p 11222 -c etc/config-samples/distributed-udp.xml", nameOfBatchFile1);
            hrServer1.StartInfo.FileName = "cmd";
            hrServer1.StartInfo.Arguments = parameters1;
            hrServer1.StartInfo.WorkingDirectory = ispnHome;
            hrServer1.Start();
            Thread.Sleep(3000);
            string parameters2 = String.Format("/k \"{0}\"" + " -r hotrod -p 11223 -c etc/config-samples/distributed-udp.xml", nameOfBatchFile2);
            hrServer2.StartInfo.FileName = "cmd";
            hrServer2.StartInfo.Arguments = parameters2;
            hrServer2.StartInfo.WorkingDirectory = ispnHome;
            hrServer2.Start();
            Thread.Sleep(3000);
            Thread.Sleep(50000);

            remoteManager1 = new RemoteCacheManager(conf1, serializer1);
        }

        [TestCleanup()]
        public void ShutDownHotrodServer()
        {
            hrServer1.CloseMainWindow();
            hrServer2.CloseMainWindow();
        }
    }
}
