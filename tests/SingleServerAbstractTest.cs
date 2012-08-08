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

namespace tests
{
    [TestClass]
    public abstract class SingleServerAbstractTest
    {
        static Process hrServer;
        static bool isServerStarted = false;
        static ClientConfig conf= new ClientConfig();
        public static Serializer s= new DefaultSerializer();
        public static RemoteCacheManager r = new RemoteCacheManager(conf, s);

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {


            if (!isServerStarted)
            {
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
                isServerStarted = true;
                Thread.Sleep(3000);
            }
        }
    }
}
