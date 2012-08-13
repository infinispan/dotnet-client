using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Util;
using System.Threading;
using Infinispan.DotNetClient.Util.Impl;

namespace tests
{
    [TestClass]
    public abstract class SingleServerAbstractTest
    {
        protected Process hrServer;
        protected ClientConfig conf= new ClientConfig();
        protected ISerializer serializer= new DefaultSerializer();
        protected RemoteCacheManager<String,String> remoteManager;

        [TestInitialize()]
        public void startHotrodServer()
        {
            //TODO - we might want to make this an actual process at some point so that the window is no longer displayed
            // http://stackoverflow.com/questions/1113000/how-do-start-stop-services-using-net-stop-command-in-c-sharp
            string ispnHome = System.Environment.GetEnvironmentVariable("ISPN_HOME");
            if (ispnHome == null)
                throw new Exception("you must set the ISPN_HOME variable pointing to the ISPN installation in order to be able to run the tests");

            hrServer = new Process();
            String nameOfBatchFile = ispnHome + "\\bin\\startServer.bat";
            string parameters = String.Format("/k \"{0}\"" + " -r hotrod", nameOfBatchFile);
            hrServer.StartInfo.FileName = "cmd";//"bin\\startServer.bat";
            hrServer.StartInfo.Arguments = parameters;
            hrServer.StartInfo.WorkingDirectory = ispnHome + "\\bin";
            //hrServer.StartInfo.Arguments = "-r hotrod";
            hrServer.Start();
            Thread.Sleep(3000);
            remoteManager = new RemoteCacheManager<String,String>(conf, serializer);
        }

        [TestCleanup()]
        public void shutDownHotrodServer()
        {
            hrServer.CloseMainWindow();
        }
    }
}
