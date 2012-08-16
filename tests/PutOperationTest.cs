using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;

namespace tests
{

    [TestClass()]
    public class PutOperationTest : SingleServerAbstractTest
    {   
        [TestMethod()]
        public void PutTest()
        {
            remoteManager.GetCache<String,String>().Put("key13", "boron");
            Assert.AreEqual("boron", remoteManager.GetCache<String,String>().Get("key13"));
            remoteManager.GetCache<String,String>().Put("key14", "chlorine");
            Assert.AreEqual("chlorine", remoteManager.GetCache<String,String>().Get("key14"));

            //If Force return value is set to true, following assertion will be passed. Else follwing will fail.
            Assert.AreEqual("chlorine", remoteManager.GetCache<String,String>().Put("key14","Berilium"));
        }
    }
}
