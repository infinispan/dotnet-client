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
                hrServer = new Process();
                String nameOfBatchFile = "C:\\Users\\User\\GSOC\\infinispan-5.1.5.FINAL\\bin\\startServer.bat";
                string parameters = String.Format("/k \"{0}\"" + " -r hotrod", nameOfBatchFile);
                hrServer.StartInfo.FileName = "cmd";//"bin\\startServer.bat";
                hrServer.StartInfo.Arguments = parameters;
                hrServer.StartInfo.WorkingDirectory = "C:\\Users\\User\\GSOC\\infinispan-5.1.5.FINAL\\bin";
                //hrServer.StartInfo.Arguments = "-r hotrod";
                hrServer.Start();
                isServerStarted = true;
                Thread.Sleep(3000);
            }
        }


    }
}
