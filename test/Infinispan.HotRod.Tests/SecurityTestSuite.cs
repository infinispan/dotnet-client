﻿using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;
using System;

namespace Infinispan.HotRod.Tests.StandaloneHotrodSSLXml
{
    [SetUpFixture]
    public class SecurityTestSuite
    {
        HotRodServer server;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            server = new HotRodServer("infinispan-ssl.xml");
            server.StartHotRodServer();
        }

        [OneTimeTearDown]
        public void AfterSuite()
        {
            server.ShutDownHotrodServer();
        }
    }
}
