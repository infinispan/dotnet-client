using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace tests
{
    [TestClass]
    public abstract class SingleServerAbstractTest
    {
        Process hrServer;

        [TestInitialize]
        public void startServer() {
            Console.WriteLine("initialize invoked!");
            //inspired from here http://www.csharp-station.com/howto/processstart.aspx
            hrServer = new Process();

            hrServer.StartInfo.FileName = "bin\\startServer.bat";
            hrServer.StartInfo.WorkingDirectory = "c:\\temp\\infinispan-5.2.0.ALPHA2-all";
            hrServer.StartInfo.Arguments = "-r hotrod";

            hrServer.Start();

            bool hasExitedExited = hrServer.HasExited;
           
            Console.WriteLine("Has exited? " + hasExitedExited);
            
        }

        [TestCleanup]
        public void stopServerHere() {
            //stop the server here
            Console.WriteLine("before kill! Has exited?" + hrServer.HasExited);
            hrServer.Kill();
            Console.WriteLine("after kill! Has exited?" + hrServer.HasExited);
        }
    }
}
