package org.infinispan.client.hotrod;

import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import static org.testng.Assert.assertEquals;

import org.infinispan.client.hotrod.BulkGetKeysDistTest;
import org.infinispan.client.hotrod.BulkGetKeysReplTest;
import org.infinispan.client.hotrod.BulkGetKeysSimpleTest;
import org.infinispan.client.hotrod.BulkGetReplTest;
import org.infinispan.client.hotrod.BulkGetSimpleTest;
import org.infinispan.client.hotrod.ClientAsymmetricClusterTest;
import org.infinispan.client.hotrod.CacheManagerStoppedTest;
import org.infinispan.client.hotrod.DefaultExpirationTest;
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

import org.testng.IMethodSelector;
import org.testng.IMethodSelectorContext;
import org.testng.ITestNGMethod;
import org.testng.ITestResult;
import org.testng.TestNG;
import org.testng.reporters.TextReporter;

import java.util.logging.*;

public class JavaClientTests implements IMethodSelector {
	private final static String [] passOverTestList = {
        "CacheManagerNotStartedTest.testPutAllAsync",
	"CacheManagerNotStartedTest.testPutAsync",
	"CacheManagerNotStartedTest.testReplaceAsync",
	"CacheManagerNotStartedTest.testVersionedRemoveAsync",
	"CacheManagerStoppedTest.testPutAllAsync",
	"CacheManagerStoppedTest.testPutAsync",
	"CacheManagerStoppedTest.testReplaceAsync",
	"CacheManagerStoppedTest.testVersionedRemoveAsync"
	};
	
   private final static HashSet<String> passOverTestSet = new HashSet<String>(Arrays.asList(passOverTestList));
	
   public static void main(String[] args) {
      TestNG testng = new TestNG();
     testng.addMethodSelector("org.infinispan.client.hotrod.JavaClientTests", 1);
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

@Override
public boolean includeMethod(IMethodSelectorContext context, ITestNGMethod method, boolean isTestMethod) {
	String testName = method.getRealClass().getSimpleName()+"."+method.getMethodName();
	if (passOverTestSet.contains(testName))
	{
		context.setStopped(true);
		return false;
	}
	return true;
}

@Override
public void setTestMethods(List<ITestNGMethod> testMethods) {
	// TODO Auto-generated method stub
	
}
}
