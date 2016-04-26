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
import org.testng.TestNG;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;
import org.testng.reporters.TextReporter;

/**
 * Tests .NET and Java Hot Rod Client interoperability.
 * 
 * @author Alan Field
 * @author Radim Vansa
 */
public class CrossLanguageHotRodTest extends SingleCacheManagerTest {
   final String DEFAULT_CACHE_MANAGER = "local";
   final String DEFAULT_CACHE = "testcache";

   private static final Log log = LogFactory.getLog(CrossLanguageHotRodTest.class);

   //Test data
   String v01 = "v0";
   String v02 = "ÅÄÇÉÑÖÕÜàäâáãçëèêéîïìíñôöòóüûùúÿ";
   String v03 = null;

   byte[] v11 = { 'v', 'a', 'l', 'u', 'e', '1' };
   boolean[] v12 = { true, false, false, true };
   char[] v13 = { 'v', 'à', 'l', 'û', 'è', '1' };
   double[] v14 = { Double.NEGATIVE_INFINITY, Double.POSITIVE_INFINITY, Double.MAX_VALUE, Double.MIN_VALUE,
         Double.MIN_NORMAL, Double.NaN, 0 };
   float[] v15 = { Float.NEGATIVE_INFINITY, Float.POSITIVE_INFINITY, Float.MAX_VALUE, Float.MIN_VALUE,
         Float.MIN_NORMAL, Float.NaN, 0 };
   int[] v16 = { Integer.MAX_VALUE, Integer.MIN_VALUE, 0 };
   long[] v17 = { Long.MAX_VALUE, Long.MIN_VALUE, 0 };
   short[] v18 = { Short.MAX_VALUE, Short.MIN_VALUE, 0 };
   String[] v19 = { "ÅÄ", "Ç", "É", "Ñ", "ÖÕ", "Ü", "àäâáã", "ç", "ëèêé", "îïìí", "ñ", "ôöòó", "üûùú", "ÿ", null };


   boolean v21 = true;
   boolean v22 = false;

   byte v31 = 127;
   byte v32 = -128;
   byte v33 = 0;

   char v41 = '4';
   char v42 = 'Ç';

   double v51 = Double.NEGATIVE_INFINITY;
   double v52 = Double.POSITIVE_INFINITY;
   double v53 = Double.MAX_VALUE;
   double v54 = Double.MIN_VALUE;
   double v55 = Double.MIN_NORMAL;
   double v56 = Double.NaN;
   double v57 = 0;

   float v61 = Float.NEGATIVE_INFINITY;
   float v62 = Float.POSITIVE_INFINITY;
   float v63 = Float.MAX_VALUE;
   float v64 = Float.MIN_VALUE;
   float v65 = Float.MIN_NORMAL;
   float v66 = Float.NaN;
   float v67 = 0;

   int v71 = Integer.MIN_VALUE;
   int v72 = Integer.MAX_VALUE;
   int v73 = 0;

   long v81 = Long.MAX_VALUE;
   long v82 = Long.MIN_VALUE;
   long v83 = 0;

   short v91 = Short.MIN_VALUE;
   short v92 = Short.MAX_VALUE;
   short v93 = 0;

   Object v10 = null;

   Object[] valueArray = { v01, v02, v03, v11, v12, v13, v14, v15, v16, v17, v18, v19, v21, v22, v31, v32, v33, v41,
         v42, v51, v52, v53, v54, v55, v56, v57, v61, v62, v63, v64, v65, v66, v67, v71, v72, v73, v81, v82, v83, v91,
         v92, v93, v10 };

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
      ((ConfigurationBuilder) config).jmxStatistics().enable();

      cacheManager = TestCacheManagerFactory.createCacheManager((ConfigurationBuilder) config);
      cacheManager.defineConfiguration(DEFAULT_CACHE, ((ConfigurationBuilder) config).build());

      hotrodServer = startHotRodServer(cacheManager);

      // this is a safer way to load the java hotrod client, without relying on the classpath
      dotnetClassLoader = new InvertedURLClassLoader(getClientURL("infinispan.client.hotrod.dotnet"));
      dotnetRemoteCacheManager = new ReflexCacheManager(dotnetClassLoader, "localhost:" + hotrodServer.getPort());
      javaClassLoader = new InvertedURLClassLoader(getClientURL("infinispan.client.hotrod.java"));
      javaRemoteCacheManager = new ReflexCacheManager(javaClassLoader, "localhost:" + hotrodServer.getPort());

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
   public void testCppPut() throws Exception {
      log.info("doCppPut()");
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
   public void testCppReplace() throws Exception {
      log.info("doCppReplace()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }

      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         if (i % 2 == 0) {
            Object replaceResult = dotnetCache.replace("k" + i, "v" + (i * 10), true);
            assertEquals(replaceResult, valueArray[i]);
         } else {
            assertNull(dotnetCache.replace("k" + i, "v" + (i * 10), false));
         }
      }
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(javaCache.get("k" + i), "v" + (i * 10));
      }
      clearCaches();
   }

   @Test
   public void testCppGet() throws Exception {
      log.info("doCppGet()");
      initEmptyCaches();

      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(dotnetCache.get("k" + i), valueArray[i]);
      }
      clearCaches();
   }

   @Test
   public void testCppGetBulk() throws Exception {
      log.info("doCppGetBulk()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);

      // getBulk(valueArray.length)
      Map<Object, Object> result = dotnetCache.getBulk(valueArray.length);
      assertEquals(result.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(result.get("k" + i), valueArray[i]);
      }

      // getBulk(0)
      result = dotnetCache.getBulk(0);
      assertEquals(result.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(result.get("k" + i), valueArray[i]);
      }

      // getBulk(valueArray.length - 1)
      result = dotnetCache.getBulk(valueArray.length - 1);
      assertEquals(result.size(), valueArray.length - 1);
      for (int i = 0; i < valueArray.length; i++) {
         if (result.containsKey("k" + i)) {
            assertEquals(result.get("k" + i), valueArray[i]);
         }
      }

      // getBulk(1)
      result = dotnetCache.getBulk(1);
      assertEquals(result.size(), 1);
      for (int i = 0; i < valueArray.length; i++) {
         if (result.containsKey("k" + i)) {
            assertEquals(result.get("k" + i), valueArray[i]);
         }
      }

      // getBulk(valueArray.length + 1)
      result = dotnetCache.getBulk(valueArray.length + 1);
      assertEquals(result.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertEquals(result.get("k" + i), valueArray[i]);
      }

      // getBulk(-1)
      result = dotnetCache.getBulk(-1);
      assertEquals(result.size(), 0);

      clearCaches();
   }

   @Test
   public void testCppRemove() throws Exception {
      log.info("doCppRemove()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         if (i % 2 == 0) {
            Object removeResult = dotnetCache.remove("k" + i, true);
            assertEquals(removeResult, valueArray[i]);
         } else {
            assertEquals(dotnetCache.remove("k" + i, false), null);
         }
         assertEquals(dotnetCache.size(), valueArray.length - (i + 1));
      }
      assertTrue(dotnetCache.isEmpty());
      assertEquals(dotnetCache.size(), 0);
   }

   @Test
   public void testCppContainsKey() throws Exception {
      log.info("doCppContainsKey()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertTrue(dotnetCache.containsKey("k" + i));
      }
      clearCaches();
   }

   @Test
   public void testCppPutIfAbsent() throws Exception {
      log.info("doCppPutIfAbsent()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertFalse(dotnetCache.containsKey("k" + i));
         assertEquals(dotnetCache.putIfAbsent("k" + i, valueArray[i], true, -1, -1), null);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         assertTrue(dotnetCache.containsKey("k" + i));
         assertEquals(javaCache.get("k" + i), valueArray[i]);
         assertEquals(dotnetCache.get("k" + i), valueArray[i]);

         if (i % 2 == 0) {
            assertEquals(valueArray[i], dotnetCache.putIfAbsent("k" + i, "newValue", true, -1, -1));
         } else {
            assertNull(dotnetCache.putIfAbsent("k" + i, "newValue", false, -1, -1));
         }
         assertEquals(javaCache.get("k" + i), valueArray[i]);
         assertEquals(dotnetCache.get("k" + i), valueArray[i]);
      }
      clearCaches();
   }

   @Test
   public void testCppLifespan() throws Exception {
      log.info("doCppLifespan()");
      long lifespanSec = 2;
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.put("k" + i, valueArray[i], false, lifespanSec, -1));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.putIfAbsent("k" + i, valueArray[i], false, lifespanSec, -1));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertNull(dotnetCache.replace("k" + i, valueArray[i], false, lifespanSec, -1));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, "javaData");
         long currentVersion = dotnetCache.getVersioned("k" + i).getVersion();
         assertTrue(dotnetCache.replaceWithVersion("k" + i, currentVersion, valueArray[i], lifespanSec, -1));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
      }
      clearCaches();
   }

   @Test
   public void testCppMaxIdle() throws Exception {
      log.info("doCppMaxIdle()");
      long maxIdleSec = 2;
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.put("k" + i, valueArray[i], false, -1, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.putIfAbsent("k" + i, valueArray[i], false, -1, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertNull(dotnetCache.replace("k" + i, valueArray[i], false, -1, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, "javaData");
         long currentVersion = dotnetCache.getVersioned("k" + i).getVersion();
         assertTrue(dotnetCache.replaceWithVersion("k" + i, currentVersion, valueArray[i], -1, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
   }

   @Test
   public void testCppLifespanAndMaxIdle() throws Exception {
      log.info("doCppLifespanAndMaxIdle()");
      long lifespanSec = 2;
      long maxIdleSec = lifespanSec / 2;
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.put("k" + i, valueArray[i], false, lifespanSec, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         assertNull(dotnetCache.putIfAbsent("k" + i, valueArray[i], false, lifespanSec, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, "javaData");
         assertNull(javaCache.replace("k" + i, valueArray[i], false, lifespanSec, maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, "javaData");
         long currentVersion = dotnetCache.getVersioned("k" + i).getVersion();

         assertTrue(dotnetCache.replaceWithVersion("k" + i, currentVersion, valueArray[i], lifespanSec,
               maxIdleSec));
         assertEquals(javaCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getLifespan(), lifespanSec);
         assertEquals(javaCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
         assertEquals(dotnetCache.getWithMetadata("k" + i).getMaxIdle(), maxIdleSec);
      }
      clearCaches();
   }

   @Test
   public void testCppStats() throws Exception {
      log.info("doCppStats()");
      initEmptyCaches();

      Map<String, String> javaStats = javaCache.getStats();
      Map<String, String> dotnetStats = dotnetCache.getStats();

      for (String key : javaStats.keySet()) {
         assertTrue(javaStats.containsKey(key));
         log.info("Key: " + key + "; Value: " + dotnetStats.get(key));
      }

      assertEquals(Long.parseLong(dotnetStats.get("currentNumberOfEntries")), 0);

      long totalNumberOfEntries = Long.parseLong(dotnetStats.get("totalNumberOfEntries"));
      long stores = Long.parseLong(dotnetStats.get("stores"));

      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         dotnetStats = dotnetCache.getStats();
         assertEquals(dotnetStats.get("currentNumberOfEntries"), String.valueOf(i + 1));
         assertEquals(dotnetStats.get("totalNumberOfEntries"), String.valueOf(totalNumberOfEntries + i + 1));
         assertEquals(dotnetStats.get("stores"), String.valueOf(stores + i + 1));
      }

      //Store initial values
      assertEquals(dotnetStats.get("currentNumberOfEntries"), String.valueOf(dotnetCache.size()));
      long hits = Long.parseLong(dotnetStats.get("hits"));
      long removeMisses = Long.parseLong(dotnetStats.get("removeMisses"));
      long removeHits = Long.parseLong(dotnetStats.get("removeHits"));
      long retrievals = Long.parseLong(dotnetStats.get("retrievals"));
      long misses = Long.parseLong(dotnetStats.get("misses"));

      //hit
      assertEquals(valueArray[0], dotnetCache.get("k0"));
      dotnetStats = dotnetCache.getStats();
      assertEquals(dotnetStats.get("hits"), String.valueOf(hits + 1));
      assertEquals(dotnetStats.get("retrievals"), String.valueOf(retrievals + 1));
      //miss
      assertNull(dotnetCache.get("NON_EXISTENT"));
      dotnetStats = dotnetCache.getStats();
      assertEquals(dotnetStats.get("misses"), String.valueOf(misses + 1));
      assertEquals(dotnetStats.get("retrievals"), String.valueOf(retrievals + 2));
      //removeHits
      assertNull(dotnetCache.remove("k0", false));
      dotnetStats = dotnetCache.getStats();
      assertEquals(dotnetStats.get("removeHits"), String.valueOf(removeHits + 1));
      assertEquals(dotnetStats.get("currentNumberOfEntries"), String.valueOf(dotnetCache.size()));
      //removeMisses
      assertNull(dotnetCache.remove("NON_EXISTENT", false));
      dotnetStats = dotnetCache.getStats();
      assertEquals(dotnetStats.get("removeMisses"), String.valueOf(removeMisses + 1));

      clearCaches();
   }

   @Test
   public void testCppReplaceWithVersion() throws Exception {
      log.info("doCppReplaceWithVersion()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }

      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         long currentVersion = dotnetCache.getVersioned("k" + i).getVersion();

         assertFalse(dotnetCache.replaceWithVersion("k" + i, currentVersion + 1, "replacedValue", -1, -1));
         assertEquals(javaCache.get("k" + i), valueArray[i]);
         assertTrue(dotnetCache.replaceWithVersion("k" + i, currentVersion, "replacedValue", -1, -1));
         assertEquals(javaCache.get("k" + i), "replacedValue");
      }
      clearCaches();
   }

   @Test
   public void testCppRemoveWithVersion() throws Exception {
      log.info("doCppRemoveWithVersion()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         long currentVersion = dotnetCache.getVersioned("k" + i).getVersion();

         assertFalse(dotnetCache.removeWithVersion("k" + i, currentVersion + 1));
         assertTrue(dotnetCache.containsKey("k" + i));
         assertTrue(dotnetCache.removeWithVersion("k" + i, currentVersion));
         assertFalse(dotnetCache.containsKey("k" + i));
         assertEquals(dotnetCache.size(), valueArray.length - (i + 1));
      }
      assertTrue(dotnetCache.isEmpty());
      assertEquals(dotnetCache.size(), 0);
   }

   @Test
   public void testCppGetWithMetadata() throws Exception {
      log.info("doCppGetWithMetadata()");
      initEmptyCaches();
      for (int i = 0; i < valueArray.length; i++) {
         javaCache.put("k" + i, valueArray[i]);
         assertEquals(dotnetCache.size(), i + 1);
      }
      assertEquals(dotnetCache.size(), valueArray.length);
      for (int i = 0; i < valueArray.length; i++) {
         ReflexMetadataValue dotnetMeta = dotnetCache.getWithMetadata("k" + i);
         ReflexMetadataValue javaMeta = javaCache.getWithMetadata("k" + i);
         // From Java: MetadataValueImpl [created=-1, lifespan=-1, lastUsed=-1, maxIdle=-1, getVersion()=1334, getValue()=v0]

         if (dotnetMeta != null) {
            assertEquals(dotnetMeta.getValue(), valueArray[i]);
            assertEquals(dotnetMeta.getCreated(), javaMeta.getCreated());
            assertEquals(dotnetMeta.getLastUsed(), javaMeta.getLastUsed());
            assertEquals(dotnetMeta.getLifespan(), javaMeta.getLifespan());
            assertEquals(dotnetMeta.getMaxIdle(), javaMeta.getMaxIdle());
            assertEquals(dotnetMeta.getVersion(), javaMeta.getVersion());
         }

      }
      clearCaches();
   }

   public static void main(String[] args) {
      TestNG testng = new TestNG();
      TextReporter tr = new TextReporter("Cross-language Test", 2);
      testng.setTestClasses(new Class[] {
         CrossLanguageHotRodTest.class
      });

      testng.addListener(tr);
      testng.run();
            String[] expectedTestFailures = { 
            // Async operations are not supported currently
      };

      assertEquals(tr.getFailedTests().size(), expectedTestFailures.length);
      System.exit(0);

   }
}
