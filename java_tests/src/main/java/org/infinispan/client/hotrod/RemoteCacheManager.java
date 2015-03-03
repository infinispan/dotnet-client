/* test jni api version */

package org.infinispan.client.hotrod;

// originals
import java.io.IOException;
import java.net.URL;
import java.util.Properties;

import org.infinispan.client.hotrod.configuration.Configuration;
import org.infinispan.client.hotrod.configuration.ConfigurationBuilder;
import org.infinispan.commons.marshall.Marshaller;
// jni wrappers

public class RemoteCacheManager /* implements BasicCacheContainer */{

    private static final String ISPN_CLIENT_HOTROD_SERVER_LIST = "infinispan.client.hotrod.server_list";

    private cli.Infinispan.HotRod.Wrappers.RemoteCacheManager jniRemoteCacheManager;
    private Marshaller marshaller = new org.infinispan.commons.marshall.jboss.GenericJBossMarshaller();

    private static Configuration asConfiguration(String servers) {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.addServers(servers);
        return builder.build();
    }

    private static Configuration asConfiguration(String server, int port) {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.addServer().host(server).port(port);
        return builder.build();
    }

    private static Properties loadProperties(URL url) throws IOException {
        Properties props = new Properties();
        props.load(url.openStream());
        return props;
    }

    public RemoteCacheManager() {
        jniRemoteCacheManager = new cli.Infinispan.HotRod.Wrappers.RemoteCacheManager();
    }

    public RemoteCacheManager(boolean start) {
        jniRemoteCacheManager = new cli.Infinispan.HotRod.Wrappers.RemoteCacheManager(start);
    }

    public RemoteCacheManager(Configuration config) {
        jniRemoteCacheManager = new cli.Infinispan.HotRod.Wrappers.RemoteCacheManager(config.getJniConfiguration());
    }

    public RemoteCacheManager(Configuration config, boolean start) {
        jniRemoteCacheManager = new cli.Infinispan.HotRod.Wrappers.RemoteCacheManager(config.getJniConfiguration(), start);
    }
    
    public RemoteCacheManager(Configuration config, boolean start, boolean useCompatibilityStringMarshaller) {
        if (useCompatibilityStringMarshaller) {
            this.marshaller = null;
        }
        jniRemoteCacheManager = new cli.Infinispan.HotRod.Wrappers.RemoteCacheManager(config.getJniConfiguration(), start, useCompatibilityStringMarshaller);
    }

    public RemoteCacheManager(String server, int port) {
        this(asConfiguration(server, port), true);
    }

    public RemoteCacheManager(String server, int port, boolean start) {
        this(asConfiguration(server, port), start);
    }

    public RemoteCacheManager(String servers) {
        this(asConfiguration(servers), true);
    }

    public RemoteCacheManager(String servers, boolean start) {
        this(asConfiguration(servers), start);
    }

    public RemoteCacheManager(URL config, boolean start) throws IOException {
        this(loadProperties(config), start);
    }
    
    public RemoteCacheManager(Properties props) {
       this(props, true);
    }

    public RemoteCacheManager(Properties props, boolean start) {
        this(new ConfigurationBuilder().withProperties(props).build(), start);
    }
    
    public <K, V> org.infinispan.client.hotrod.RemoteCache<K, V> getCache() {
        cli.Infinispan.HotRod.Wrappers.RemoteCache cache = getJniRemoteCacheManager().GetCache();
        return new org.infinispan.client.hotrod.impl.RemoteCacheImpl<K, V>(this, cache);
    }

    public <K, V> org.infinispan.client.hotrod.RemoteCache<K, V> getCache(boolean forceReturnValue) {
        cli.Infinispan.HotRod.Wrappers.RemoteCache cache = getJniRemoteCacheManager().GetCache(forceReturnValue);
        return new org.infinispan.client.hotrod.impl.RemoteCacheImpl<K, V>(this, cache);
    }

    public <K, V> org.infinispan.client.hotrod.RemoteCache<K, V> getCache(String cacheName) {
        cli.Infinispan.HotRod.Wrappers.RemoteCache cache = getJniRemoteCacheManager().GetCache(cacheName);
        return new org.infinispan.client.hotrod.impl.RemoteCacheImpl<K, V>(this, cache);
    }

    public <K, V> org.infinispan.client.hotrod.RemoteCache<K, V> getCache(String cacheName, boolean forceReturnValue) {
        cli.Infinispan.HotRod.Wrappers.RemoteCache cache = getJniRemoteCacheManager().GetCache(cacheName, forceReturnValue);
        return new org.infinispan.client.hotrod.impl.RemoteCacheImpl<K, V>(this, cache);
    }

    public Marshaller getMarshaller() {
        return marshaller;
    }

    public cli.Infinispan.HotRod.Wrappers.RemoteCacheManager getJniRemoteCacheManager() {
        return jniRemoteCacheManager;
    }
    
    public boolean isStarted() {
        return jniRemoteCacheManager.IsStarted();
    }
    
    public void start() {
        jniRemoteCacheManager.Start();
    }
    
    public void stop() {
        jniRemoteCacheManager.Stop();
    }
    
    public Properties getProperties() {
        throw new UnsupportedOperationException();
    }
}
