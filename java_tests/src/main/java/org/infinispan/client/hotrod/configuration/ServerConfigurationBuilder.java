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

   ServerConfigurationBuilder(ConfigurationBuilder builder) {
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

   public ServerConfigurationBuilder read(ServerConfiguration template) {      
      this.jniServerConfigurationBuilder.Host(template.host());
      this.jniServerConfigurationBuilder.Port(template.port());

      return this;
   }

}
