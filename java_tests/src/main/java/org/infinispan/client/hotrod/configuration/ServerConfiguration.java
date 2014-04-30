package org.infinispan.client.hotrod.configuration;

/**
 * ServerConfiguration.
 *
 * @author Tristan Tarrant
 * @since 5.3
 */
public class ServerConfiguration {
   private cli.Infinispan.HotRod.Config.ServerConfiguration jniServerConfiguration;

   ServerConfiguration(cli.Infinispan.HotRod.Config.ServerConfiguration jniServerConfiguration) {
      this.jniServerConfiguration = jniServerConfiguration;
   }

   public String host() {
      return this.jniServerConfiguration.Host();
   }

   public int port() {
      return this.jniServerConfiguration.Port();
   }

}
