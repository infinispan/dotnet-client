using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient;
using System.Threading;
using Infinispan.DotNetClient.Util.Impl;

namespace tests
{
    [TestClass]
    public class MultipleServerAbstractTest
    {
        protected Process hrServer1;
        protected ClientConfig conf1 = new ClientConfig("127.0.0.1", 11222, "cache1", false);
        protected ISerializer serializer1 = new DefaultSerializer();
        protected RemoteCacheManager<String, String> remoteManager1;

        protected Process hrServer2;
        protected ClientConfig conf2 = new ClientConfig("127.0.0.1", 11223, "cache2", false);
        protected ISerializer serializer2 = new DefaultSerializer();
        protected RemoteCacheManager<String, String> remoteManager2;

        [TestInitialize()]
        public void startHotrodServer()
        {
            //TODO - we might want to make this an actual process at some point so that the window is no longer displayed
            // http://stackoverflow.com/questions/1113000/how-do-start-stop-services-using-net-stop-command-in-c-sharp
            string ispnHome = System.Environment.GetEnvironmentVariable("ISPN_HOME");
            if (ispnHome == null)
                throw new Exception("you must set the ISPN_HOME variable pointing to the ISPN installation in order to be able to run the tests");

            hrServer1 = new Process();
            String nameOfBatchFile1 = ispnHome + "\\bin\\startServer.bat";
            String nameOfBatchFile2 = ispnHome + "\\bin\\startServer.bat";
            string parameters1 = String.Format("/k \"{0}\"" + " -r hotrod", nameOfBatchFile1);
            hrServer1.StartInfo.FileName = "cmd";
            hrServer1.StartInfo.Arguments = parameters1;
            hrServer1.StartInfo.WorkingDirectory = ispnHome + "\\bin";
            string parameters2 = String.Format("/k \"{0}\"" + " -r hotrod", nameOfBatchFile2);
            hrServer2.StartInfo.FileName = "cmd";
            hrServer2.StartInfo.Arguments = parameters2;
            hrServer2.StartInfo.WorkingDirectory = ispnHome + "\\bin";
            hrServer2.Start();
            Thread.Sleep(3000);
            remoteManager1 = new RemoteCacheManager<String, String>(conf1, serializer1);
            remoteManager1 = new RemoteCacheManager<String, String>(conf2, serializer2);
        }

        [TestCleanup()]
        public void shutDownHotrodServer()
        {
            hrServer1.CloseMainWindow();
        }
    }
}
