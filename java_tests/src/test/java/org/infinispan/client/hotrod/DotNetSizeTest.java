package org.infinispan.client.hotrod;

public class DotNetSizeTest extends SizeTest {
	   protected org.infinispan.client.hotrod.configuration.ConfigurationBuilder createHotRodClientConfigurationBuilder(int serverPort) {
		      org.infinispan.client.hotrod.configuration.ConfigurationBuilder clientBuilder = new org.infinispan.client.hotrod.configuration.ConfigurationBuilder();
		      clientBuilder.addServer()
		            .host("localhost")
		            .port(serverPort);
		      clientBuilder
		            .maxRetries(maxRetries());
		      return clientBuilder;
		   }

}
