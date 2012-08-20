using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
    /// <summary>
    /// Summary description for MultipleServerPutTest
    /// </summary>
    [TestClass]
    public class MultipleServerPutTest:MultipleServerAbstractTest
    {
        [TestMethod()]
        public void MultipleServerPutOperationTest()
        {
            remoteManager1.GetCache<String, String>().Put("key13", "boron");//Put value to remoteCache1
            Assert.AreEqual("boron", remoteManager1.GetCache<String, String>().Get("key13"));//Request the value put into remotecache1 from remotecache2
            remoteManager1.GetCache<String, String>().Put("key14", "chlorine");
            Assert.AreEqual("chlorine", remoteManager1.GetCache<String, String>().Get("key14"));
            remoteManager1.GetCache<String, String>().Put("key14", "chlorine");
            Assert.AreEqual("chlorine", remoteManager1.GetCache<String, String>().Get("key14"));
        }
    }
}
