package org.infinispan.client.hotrod;
import org.infinispan.util.logging.Log;
import org.infinispan.client.hotrod.test.HotRodClientTestingUtil;
import org.infinispan.client.hotrod.ServerRestartTest;
import org.infinispan.client.hotrod.configuration.ConfigurationBuilder;
import org.infinispan.server.hotrod.HotRodServer;
import org.infinispan.server.hotrod.configuration.HotRodServerConfigurationBuilder;
import org.infinispan.util.logging.LogFactory;
/** This class overrides the setup configuration
 * of the test org.infinispan.client.hotrod.ServerRestartTest. Configuration framework has been simplified
 * in the C++ client and ConfigurationBuilder properties cannot be set via the ServerConfigurationBuilder.
 * This code is equivalent to the original one.
 */
public class DotNetServerRestartTest extends ServerRestartTest
{
private static final Log log = LogFactory.getLog(DotNetServerRestartTest.class);
   private RemoteCache<String, String> defaultRemote;
   private RemoteCacheManager remoteCacheManager;

   protected HotRodServer hotrodServer;
   @Override
   protected void setup() throws Exception {
      cacheManager = createCacheManager();
      if (cache == null) cache = cacheManager.getCache();
      hotrodServer = HotRodClientTestingUtil.startHotRodServer(cacheManager);
      log.info("Started server on port: " + hotrodServer.getPort());

      ConfigurationBuilder builder = new ConfigurationBuilder();
      builder.addServer().host("127.0.0.1").port(hotrodServer.getPort());
      builder.connectionPool().timeBetweenEvictionRuns(2000);
      remoteCacheManager = new RemoteCacheManager(builder.build());
      defaultRemote = remoteCacheManager.getCache();
   }
   @Override
   public void testServerShutdown() throws Exception {
	      defaultRemote.put("k","v");
	      assert defaultRemote.get("k").equals("v");

	      int port = hotrodServer.getPort();
	      hotrodServer.stop();

	      HotRodServerConfigurationBuilder builder = new HotRodServerConfigurationBuilder();
	      builder.host("127.0.0.1").port(port).workerThreads(2).idleTimeout(20000).tcpNoDelay(true).sendBufSize(15000).recvBufSize(25000);
	      hotrodServer.start(builder.build(), cacheManager);

	      Thread.sleep(3000);

	      assert defaultRemote.get("k").equals("v");
	      defaultRemote.put("k","v");
	   }

}
