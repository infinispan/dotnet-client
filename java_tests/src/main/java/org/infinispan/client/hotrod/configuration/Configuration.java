package org.infinispan.client.hotrod.configuration;

import java.lang.ref.WeakReference;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

import org.infinispan.client.hotrod.impl.consistenthash.ConsistentHash;
import org.infinispan.client.hotrod.impl.transport.TransportFactory;
import org.infinispan.client.hotrod.impl.transport.tcp.RequestBalancingStrategy;
import org.infinispan.client.hotrod.configuration.ConnectionPoolConfiguration;
import org.infinispan.client.hotrod.configuration.SslConfiguration;
import org.infinispan.commons.configuration.BuiltBy;
import org.infinispan.commons.marshall.Marshaller;

/**
 * Configuration.
 *
 * @author Tristan Tarrant
 * @since 5.3
 */
@BuiltBy(ConfigurationBuilder.class)
public class Configuration {

   private final ExecutorFactoryConfiguration asyncExecutorFactory;
   private final Class<? extends RequestBalancingStrategy> balancingStrategy;
   private final WeakReference<ClassLoader> classLoader;
   private final Class<? extends ConsistentHash>[] consistentHashImpl;
   private final Class<? extends Marshaller> marshallerClass;
   private final Marshaller marshaller;
   private final List<ServerConfiguration> servers;
   private final Class<? extends TransportFactory> transportFactory;
   
   private cli.Infinispan.HotRod.Config.Configuration jniConfiguration;

   public Configuration(cli.Infinispan.HotRod.Config.Configuration jniConfiguration) {
      super();
      this.jniConfiguration = jniConfiguration;
      this.asyncExecutorFactory = null;
      this.balancingStrategy = null;
      this.classLoader = null;
      this.consistentHashImpl = null;
      this.marshallerClass = null;
      this.marshaller = null;
      this.servers = null;
      this.transportFactory = null;
   }

   Configuration(ExecutorFactoryConfiguration asyncExecutorFactory, Class<? extends RequestBalancingStrategy> balancingStrategy, ClassLoader classLoader,
         ConnectionPoolConfiguration connectionPool, int connectionTimeout, Class<? extends ConsistentHash>[] consistentHashImpl, boolean forceReturnValues, int keySizeEstimate, Class<? extends Marshaller> marshallerClass,
         boolean pingOnStartup, String protocolVersion, List<ServerConfiguration> servers, int socketTimeout, SslConfiguration ssl, boolean tcpNoDelay,
         Class<? extends TransportFactory> transportFactory, int valueSizeEstimate) {
       this.jniConfiguration = null; // new cli.Infinispan.HotRod.Config.ConfigurationBuilder().Build();
      this.asyncExecutorFactory = asyncExecutorFactory;
      this.balancingStrategy = balancingStrategy;
      this.classLoader = new WeakReference<ClassLoader>(classLoader);
      this.consistentHashImpl = consistentHashImpl;
      this.marshallerClass = marshallerClass;
      this.marshaller = null;
      this.servers = Collections.unmodifiableList(servers);
      this.transportFactory = transportFactory;
   }

   Configuration(ExecutorFactoryConfiguration asyncExecutorFactory, Class<? extends RequestBalancingStrategy> balancingStrategy, ClassLoader classLoader,
         ConnectionPoolConfiguration connectionPool, int connectionTimeout, Class<? extends ConsistentHash>[] consistentHashImpl, boolean forceReturnValues, int keySizeEstimate, Marshaller marshaller,
         boolean pingOnStartup, String protocolVersion, List<ServerConfiguration> servers, int socketTimeout, SslConfiguration ssl, boolean tcpNoDelay,
         Class<? extends TransportFactory> transportFactory, int valueSizeEstimate) {
       this.jniConfiguration = null; // new cli.Infinispan.HotRod.Config.ConfigurationBuilder().Build();
      this.asyncExecutorFactory = asyncExecutorFactory;
      this.balancingStrategy = balancingStrategy;
      this.classLoader = new WeakReference<ClassLoader>(classLoader);
      this.consistentHashImpl = consistentHashImpl;
      this.marshallerClass = null;
      this.marshaller = marshaller;
      this.servers = Collections.unmodifiableList(servers);
      this.transportFactory = transportFactory;
   }

   public cli.Infinispan.HotRod.Config.Configuration getJniConfiguration() {
      return jniConfiguration;
   }

   public ExecutorFactoryConfiguration asyncExecutorFactory() {
      return asyncExecutorFactory;
   }

   public Class<? extends RequestBalancingStrategy> balancingStrategy() {
      return balancingStrategy;
   }

   public ClassLoader classLoader() {
      return classLoader.get();
   }

   public ConnectionPoolConfiguration connectionPool() {
       throw new UnsupportedOperationException();
      // return new ConnectionPoolConfiguration(this.jniConfiguration.ConnectionPool());
   }

   public int connectionTimeout() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.ConnectionTimeout();
   }

   public Class<? extends ConsistentHash>[] consistentHashImpl() {
      return consistentHashImpl;
   }

   public Class<? extends ConsistentHash> consistentHashImpl(int version) {
      return consistentHashImpl[version-1];
   }

   public boolean forceReturnValues() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.ForceReturnValues();
   }

   public int keySizeEstimate() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.KeySizeEstimate();
   }

   public Marshaller marshaller() {
      return marshaller;
   }

   public Class<? extends Marshaller> marshallerClass() {
      return marshallerClass;
   }

   public boolean pingOnStartup() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.PingOnStartup();
   }

   public String protocolVersion() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.ProtocolVersion();
   }

   public List<ServerConfiguration> servers() {
      return servers;
   }

   public int socketTimeout() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.SocketTimeout();
   }

   public SslConfiguration ssl() {
       throw new UnsupportedOperationException();
      // return new SslConfiguration(this.jniConfiguration.Ssl());
   }

   public boolean tcpNoDelay() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.TcpNoDelay();
   }

   public Class<? extends TransportFactory> transportFactory() {
      return transportFactory;
   }

   public int valueSizeEstimate() {
       throw new UnsupportedOperationException();
      // return this.jniConfiguration.ValueSizeEstimate();
   }

   public int maxRetries() {
       throw new UnsupportedOperationException();
      //return this.jniConfiguration.getMaxRetries();
   }

   @Override
   public String toString() {
      return "Configuration [asyncExecutorFactory=" + asyncExecutorFactory + ", balancingStrategy=" + balancingStrategy + ", classLoader=" + classLoader + ", connectionPool="
            + connectionPool() + ", connectionTimeout=" + connectionTimeout() + ", consistentHashImpl=" + Arrays.toString(consistentHashImpl) + ", forceReturnValues="
            + forceReturnValues() + ", keySizeEstimate=" + keySizeEstimate() + ", marshallerClass=" + marshallerClass + ", marshaller=" + marshaller + ", pingOnStartup="
            + pingOnStartup() + ", protocolVersion=" + protocolVersion() + ", servers=" + servers + ", socketTimeout=" + socketTimeout() + ", ssl=" + ssl() + ", tcpNoDelay=" + tcpNoDelay()
            + ", transportFactory=" + transportFactory + ", valueSizeEstimate=" + valueSizeEstimate() + "]";
   }
}
