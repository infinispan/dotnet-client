package org.infinispan.client.hotrod.configuration;

import org.infinispan.commons.configuration.Builder;

/**
 * ServerConfigurationBuilder.
 *
 * @author Tristan Tarrant
 * @since 5.3
 */
public class ServerConfigurationBuilder {
   private cli.Infinispan.HotRod.Config.ServerConfigurationBuilder jniServerConfigurationBuilder;
   private ConfigurationBuilder builder;

   ServerConfigurationBuilder(ConfigurationBuilder builder) {
      this.builder = builder;  
      jniServerConfigurationBuilder = builder.getJniConfigurationBuilder().AddServer();
   }

   public ServerConfigurationBuilder host(String host) {
      this.jniServerConfigurationBuilder.Host(host);
      return this;
   }

   public ServerConfigurationBuilder port(int port) {
      this.jniServerConfigurationBuilder.Port(port);
      return this;
   }

   public ServerConfiguration create() {
      return new ServerConfiguration(this.jniServerConfigurationBuilder.Create());
   }

   public ConfigurationBuilder maxRetries(int maxRetries) {
      this.jniServerConfigurationBuilder.MaxRetries(maxRetries);
      return this.builder;
   }

   public ConnectionPoolConfigurationBuilder connectionPool() {
      return new ConnectionPoolConfigurationBuilder(this.builder,this.jniServerConfigurationBuilder.ConnectionPool());
   }

   public ServerConfigurationBuilder read(ServerConfiguration template) {      
      this.jniServerConfigurationBuilder.Host(template.host());
      this.jniServerConfigurationBuilder.Port(template.port());

      return this;
   }

}
