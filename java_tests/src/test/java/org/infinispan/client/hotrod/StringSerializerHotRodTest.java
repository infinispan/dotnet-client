package org.infinispan.client.hotrod;

import static org.infinispan.client.hotrod.test.HotRodClientTestingUtil.killServers;
import static org.infinispan.client.hotrod.test.HotRodClientTestingUtil.startHotRodServer;
import static org.infinispan.server.hotrod.test.HotRodTestingUtil.hotRodCacheConfiguration;
import static org.infinispan.test.TestingUtil.killCacheManagers;
import static org.testng.Assert.*;

import java.io.File;
import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.net.MalformedURLException;
import java.net.URI;
import java.net.URL;
import java.util.*;

import org.infinispan.client.hotrod.logging.Log;
import org.infinispan.client.hotrod.logging.LogFactory;
import org.infinispan.configuration.cache.ConfigurationBuilder;
import org.infinispan.manager.EmbeddedCacheManager;
import org.infinispan.server.hotrod.HotRodServer;
import org.infinispan.test.SingleCacheManagerTest;
import org.infinispan.test.fwk.TestCacheManagerFactory;
import org.infinispan.client.hotrod.AnyServerEquivalence;
import org.testng.IMethodSelector;
import org.testng.IMethodSelectorContext;
import org.testng.ITestNGMethod;
import org.testng.ITestResult;
import org.testng.TestNG;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;
import org.testng.reporters.TextReporter;

/**
 * Tests StringSerializer on .NET side for easy interoperability between Java and .NET client when
 * string keys and values are used. 
 * 
 * @author Martin Gencur
  */
public class StringSerializerHotRodTest extends SingleCacheManagerTest implements IMethodSelector {
   final String DEFAULT_CACHE_MANAGER = "local";
   final String DEFAULT_CACHE = "testcache";

   private final AnyServerEquivalence EQUIVALENCE = new AnyServerEquivalence();
   private static final Log log = LogFactory.getLog(CrossLanguageHotRodTest.class);

   //Test data
   String v01 = "v0";
   String v02 = "≈ƒ«…—÷’‹‡‰‚·„ÁÎËÍÈÓÔÏÌÒÙˆÚÛ¸˚˘˙ˇ";

   Object[] valueArray = { v01, v02 };

   String serverConfigPath = System.getProperty("server1.dist") + File.separator + "standalone" + File.separator
         + "configuration";

   private HotRodServer hotrodServer;
   private InvertedURLClassLoader dotnetClassLoader;
   private InvertedURLClassLoader javaClassLoader;
   private ReflexCacheManager dotnetRemoteCacheManager;
   private ReflexCacheManager javaRemoteCacheManager;
   private ReflexCache dotnetCache;
   private ReflexCache javaCache;

   @Override
   protected EmbeddedCacheManager createCacheManager() throws Exception {
      // Enable statistics in the global configuration
      Object config = hotRodCacheConfiguration();
      ((ConfigurationBuilder) config).jmxStatistics().enable()
        .compatibility().enable()
        .dataContainer().keyEquivalence(EQUIVALENCE).valueEquivalence(EQUIVALENCE);

      cacheManager = TestCacheManagerFactory.createCacheManager((ConfigurationBuilder) config);
      cacheManager.defineConfiguration(DEFAULT_CACHE, ((ConfigurationBuilder) config).build());

      hotrodServer = startHotRodServer(cacheManager);

      // this is a safer way to load the java hotrod client, without relying on the classpath
      dotnetClassLoader = new InvertedURLClassLoader(getClientURL("infinispan.client.hotrod.dotnet"));
      boolean useCompatibilityStringSerializer = true;
      //use CompatibilitySerializer on .NET side
      dotnetRemoteCacheManager = new ReflexCacheManager(dotnetClassLoader, "localhost:" + hotrodServer.getPort(), useCompatibilityStringSerializer);
      javaClassLoader = new InvertedURLClassLoader(getClientURL("infinispan.client.hotrod.java"));
      //do NOT use the serializer on Java side
      useCompatibilityStringSerializer = false;
      javaRemoteCacheManager = new ReflexCacheManager(javaClassLoader, "localhost:" + hotrodServer.getPort(), useCompatibilityStringSerializer);

      return cacheManager;
   }

   private URL[] getClientURL(String property) throws MalformedURLException {
      File target = new File(System.getProperty(property));
      if (!target.exists()) throw new IllegalStateException(target + " does not exist!");
      if (target.isDirectory()) {
         File[] files = target.listFiles();
         if (files.length != 1) throw new IllegalStateException(target + " does not contain single file!");
         target = files[0];
      }
      return new URL[] { target.toURI().toURL() };
   }


   @AfterMethod(alwaysRun=true)
   public void release() {
      killCacheManagers(cacheManager);
      try {
         if (dotnetRemoteCacheManager != null) {
            dotnetRemoteCacheManager.stop();
         }
      } catch (Exception e) {
         log.error("Failed to stop dotnet RCM", e);
      }
      try {
         if (javaRemoteCacheManager != null) {
            javaRemoteCacheManager.stop();
         }
      } catch (Exception e) {
         log.error("Failed to stop dotnet RCM", e);
      }
      killServers(hotrodServer);
      //Close the ClassLoader or the JVM process won't exit
      if (dotnetClassLoader != null) {
         try {
            dotnetClassLoader.close();
         } catch (IOException e) {
            e.printStackTrace();
         }
      }
      if (javaClassLoader != null) {
         try {
            javaClassLoader.close();
         } catch (IOException e) {
            e.printStackTrace();
         }
      }
   }

   @BeforeMethod
   public void setUp() throws Exception {
      log.info("setUp()");
      this.setup();
   }

   private void initEmptyCaches() throws InvocationTargetException, IllegalAccessException, NoSuchMethodException, ClassNotFoundException {
      dotnetCache = dotnetRemoteCacheManager.getCache(DEFAULT_CACHE);
      javaCache = javaRemoteCacheManager.getCache(DEFAULT_CACHE);

      assertTrue(dotnetRemoteCacheManager.isStarted());
      assertTrue(javaRemoteCacheManager.isStarted());
      assertTrue(dotnetCache.isEmpty());
      assertTrue(javaCache.isEmpty());
      assertEquals(dotnetCache.size(), 0);
      assertEquals(javaCache.size(), 0);
   }

   private void clearCaches() throws InvocationTargetException, IllegalAccessException {
      dotnetCache.clear();
      assertTrue(dotnetCache.isEmpty());
      assertTrue(javaCache.isEmpty());
      assertEquals(dotnetCache.size(), 0);
      assertEquals(javaCache.size(), 0);
   }

   /*
    * test methods
    */

   @Test
   public void testDotNetPut() throws Exception {
      log.info("doDotNetPut()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.put("k" + i, valueArray[i], true, -1, -1));
         assertEquals(dotnetCache.size(), i + 1);
      }

      assertEquals(dotnetCache.size(), valueArray.length);

      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(javaCache.get("k" + i), valueArray[i]);
      }

      dotnetCache.clear();
      assertTrue(dotnetCache.isEmpty());
      assertTrue(javaCache.isEmpty());
      assertEquals(dotnetCache.size(), 0);
      assertEquals(javaCache.size(), 0);
   }

   @Test
   public void testDotNetGet() throws Exception {
      log.info("doDotNetGet()");
      initEmptyCaches();

      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
      }
      
      assertEquals(dotnetCache.size(), valueArray.length);
      
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(dotnetCache.get("k" + i), valueArray[i]);
      }
      clearCaches();
   }

   private final static String [] passOverTestList = {
	};
   private final static HashSet<String> passOverTestSet = new HashSet<String>(Arrays.asList(passOverTestList));

   public static void main(String[] args) {
      TestNG testng = new TestNG();
     testng.addMethodSelector("org.infinispan.client.hotrod.StringSerializerHotRodTest", 1);
      TextReporter tr = new TextReporter("StringSerializer Test", 2);
      testng.setTestClasses(new Class[] {
         StringSerializerHotRodTest.class
      });

      testng.addListener(tr);
      testng.run();
      Set<String> expectedTestFailures = new TreeSet<String>(Arrays.asList( 
			      "StringSerializerHotRodTest.testDotNetGet"
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


