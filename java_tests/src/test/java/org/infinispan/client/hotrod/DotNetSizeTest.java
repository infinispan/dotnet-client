package org.infinispan.client.hotrod;

/** This class overrides the setup configuration
 * of the test org.infinispan.client.hotrod.SizeTest. Configuration framework has been simplified
 * in the C++ client and maxRetries which is a ConfigurationBuilder property cannot be set via the ServerConfigurationBuilder.
 * This code is equivalent to the original one.
 */
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
