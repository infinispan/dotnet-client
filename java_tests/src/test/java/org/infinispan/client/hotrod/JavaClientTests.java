package org.infinispan.client.hotrod;

import static org.testng.Assert.assertEquals;

import org.infinispan.client.hotrod.BulkGetKeysDistTest;
import org.infinispan.client.hotrod.BulkGetKeysReplTest;
import org.infinispan.client.hotrod.BulkGetKeysSimpleTest;
import org.infinispan.client.hotrod.BulkGetReplTest;
import org.infinispan.client.hotrod.BulkGetSimpleTest;
import org.infinispan.client.hotrod.ClientAsymmetricClusterTest;
import org.infinispan.client.hotrod.CacheManagerStoppedTest;
import org.infinispan.client.hotrod.DefaultExpirationTest;
import org.infinispan.client.hotrod.ForceReturnValueTest;
import org.infinispan.client.hotrod.ForceReturnValuesTest;
import org.infinispan.client.hotrod.HotRodIntegrationTest;
import org.infinispan.client.hotrod.HotRodServerStartStopTest;
import org.infinispan.client.hotrod.HotRodStatisticsTest;
import org.infinispan.client.hotrod.RemoteCacheManagerTest;
import org.infinispan.client.hotrod.ServerErrorTest;
import org.infinispan.client.hotrod.ServerRestartTest;
import org.infinispan.client.hotrod.ServerShutdownTest;
import org.infinispan.client.hotrod.SocketTimeoutErrorTest;
import org.infinispan.client.hotrod.retry.ServerFailureRetryTest;
import org.testng.TestNG;
import org.testng.reporters.TextReporter;

import java.util.logging.*;

public class JavaClientTests {
   public static void main(String[] args) {
      TestNG testng = new TestNG();
      TextReporter tr = new TextReporter("Java Tests", 2);

      if (Boolean.parseBoolean(System.getProperty("VERBOSE_HOTROD_JAVA_TESTS"))) {
          Logger.getLogger("").setLevel(Level.ALL);
          for (Handler handler : Logger.getLogger("").getHandlers()) {
              handler.setLevel(java.util.logging.Level.ALL);
          }
          testng.setVerbose(10);
      }

      testng.setTestClasses(new Class[] {
//            RemoteCacheManagerTest.class,
//            ClientAsymmetricClusterTest.class,
              // ServerFailureRetryTest.class,

            //Known to work
            BulkGetKeysDistTest.class, 
            BulkGetKeysReplTest.class, 
            BulkGetKeysSimpleTest.class,
            BulkGetReplTest.class,
            BulkGetSimpleTest.class, 
            DefaultExpirationTest.class,
            CacheManagerStoppedTest.class,
            ForceReturnValuesTest.class, 
            ForceReturnValueTest.class, 
            HotRodIntegrationTest.class,
            HotRodServerStartStopTest.class, 
            HotRodStatisticsTest.class, 
            ServerErrorTest.class,
            ServerRestartTest.class,
            ServerShutdownTest.class,
            SocketTimeoutErrorTest.class,
      });

      testng.addListener(tr);
      testng.run();

      String[] expectedTestFailures = { 
            // Async operations are not supported currently
            "HotRodIntegrationTest.testReplaceWithVersionWithLifespanAsync",
            "CacheManagerStoppedTest.testPutAllAsync",
            "CacheManagerStoppedTest.testPutAsync",
            "CacheManagerStoppedTest.testReplaceAsync",
            "CacheManagerStoppedTest.testVersionedRemoveAsync",
      };

      assertEquals(tr.getFailedTests().size(), expectedTestFailures.length);
      System.exit(0);
   }
}
