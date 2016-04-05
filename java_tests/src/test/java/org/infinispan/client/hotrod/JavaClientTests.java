package org.infinispan.client.hotrod;

import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.TreeSet;

import static org.testng.Assert.assertEquals;

import org.infinispan.client.hotrod.BulkGetKeysDistTest;
import org.infinispan.client.hotrod.BulkGetKeysReplTest;
import org.infinispan.client.hotrod.BulkGetKeysSimpleTest;
import org.infinispan.client.hotrod.BulkGetReplTest;
import org.infinispan.client.hotrod.BulkGetSimpleTest;
import org.infinispan.client.hotrod.ClientAsymmetricClusterTest;
import org.infinispan.client.hotrod.CacheManagerStoppedTest;
import org.infinispan.client.hotrod.CacheManagerNotStartedTest;
import org.infinispan.client.hotrod.DefaultExpirationTest;
import org.infinispan.client.hotrod.ForceReturnValuesTest;
import org.infinispan.client.hotrod.HotRodIntegrationTest;
import org.infinispan.client.hotrod.HotRodServerStartStopTest;
import org.infinispan.client.hotrod.HotRodStatisticsTest;
import org.infinispan.client.hotrod.RemoteCacheManagerTest;
import org.infinispan.client.hotrod.ServerErrorTest;
import org.infinispan.client.hotrod.ServerRestartTest;
import org.infinispan.client.hotrod.ServerShutdownTest;
import org.infinispan.client.hotrod.SizeTest;
import org.infinispan.client.hotrod.SocketTimeoutErrorTest;
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
	"CacheManagerStoppedTest.testVersionedRemoveAsync",
        "HotRodIntegrationTest.testReplaceWithVersionWithLifespanAsync"
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
            CacheManagerNotStartedTest.class,
            RemoteCacheManagerTest.class,
            
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
            RemoteCacheManagerTest.class,
            ServerErrorTest.class,
            ServerRestartTest.class,
            //ServerShutdownTest.class,
            SizeTest.class,
            SocketTimeoutErrorTest.class,
      });

      testng.addListener(tr);
      testng.setGroups("unit,functional");
      testng.run();

      Set<String> expectedTestFailures = new TreeSet<String>(Arrays.asList( 
            //deprecated in the Java client, and not available in C# client  
            "RemoteCacheManagerTest.testUrlAndBooleanConstructor",
            //see HRCPP-190
            "RemoteCacheManagerTest.testMarshallerInstance",
            //see HRCPP-189
            "RemoteCacheManagerTest.testGetUndefinedCache",
	    "ForceReturnValuesTest.testDifferentInstancesForDifferentForceReturnValues",
 	    "ForceReturnValuesTest.testSameInstanceForSameForceReturnValues"
      ));
      Set<String> expectedSkips = Collections.emptySet();

      Set<String> failures = new TreeSet<String>();
      for (ITestResult failed : tr.getFailedTests()) {
         failures.add(failed.getTestClass().getRealClass().getSimpleName() + "." + failed.getMethod().getMethodName());
      }
      Set<String> skips = new TreeSet<String>();
      for (ITestResult skipped : tr.getSkippedTests()) {
         failures.add(skipped.getTestClass().getRealClass().getSimpleName() + "." + skipped.getMethod().getMethodName());
      }

      int exitCode = 0;
      
      Set<String> unexpectedFails = new TreeSet<String>(failures);
      unexpectedFails.removeAll(expectedTestFailures);
      if (!unexpectedFails.isEmpty()) {
         exitCode = 1;
         System.err.println("These test fail (but should not!):");
	 for (String testName : unexpectedFails) {
            System.err.println("\t" + testName);
         } 
      }
      Set<String> notFailing = new TreeSet<String>(expectedTestFailures);
      notFailing.removeAll(failures);
      if (!notFailing.isEmpty()) {
         exitCode = 1;
         System.err.println("These test should fail (but don't!):");
	 for (String testName : notFailing) {
            System.err.println("\t" + testName);
         }
      }
      Set<String> unexpectedSkips = new TreeSet<String>(skips);
      unexpectedSkips.removeAll(expectedSkips);
      if (!unexpectedSkips.isEmpty()) {
         exitCode = 1;
         System.err.println("These test have been skipped (but should not!):");
	 for (String testName : unexpectedSkips) {
            System.err.println("\t" + testName);
         } 
      }
      Set<String> notSkipped = new TreeSet<String>(expectedSkips);
      notSkipped.removeAll(skips);
      if (!notSkipped.isEmpty()) {
         exitCode = 1;
         System.err.println("These test should have been skipped (but haven't!):");
	 for (String testName : notSkipped) {
            System.err.println("\t" + testName);
         }
      }

      /* Force exit when tests pass also as some of the tests expected to fail
         might not properly clean-up and as a result the process will not terminate
         when main() returns. */
      System.exit(exitCode);
      

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
