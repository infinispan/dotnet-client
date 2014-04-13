package org.infinispan.client.hotrod.configuration;

/**
 * ConnectionPoolConfiguration.
 *
 * @author Tristan Tarrant
 * @since 5.3
 */
public class ConnectionPoolConfiguration {
   private ExhaustedAction exhaustedAction = ExhaustedAction.WAIT;
   
   private cli.Infinispan.HotRod.Config.ConnectionPoolConfiguration jniConnectionPoolConfiguration;

   public ConnectionPoolConfiguration(
         cli.Infinispan.HotRod.Config.ConnectionPoolConfiguration jniConnectionPoolConfiguration) {
      this.jniConnectionPoolConfiguration = jniConnectionPoolConfiguration;
      // if (jniConnectionPoolConfiguration != null) {
      //    this.exhaustedAction.setExhaustedAction(this.jniConnectionPoolConfiguration.ExhaustedAction());
      // }
   }
   
   public cli.Infinispan.HotRod.Config.ConnectionPoolConfiguration getJniConnectionPoolConfiguration() {
      return this.jniConnectionPoolConfiguration;
   }

   public ExhaustedAction exhaustedAction() {
      // this.exhaustedAction.setExhaustedAction(this.jniConnectionPoolConfiguration.ExhaustedAction());
      return this.exhaustedAction;
   }

   public boolean lifo() {
      return this.jniConnectionPoolConfiguration.Lifo();
   }

   public int maxActive() {
      return this.jniConnectionPoolConfiguration.MaxActive();
   }

   public int maxTotal() {
      return this.jniConnectionPoolConfiguration.MaxTotal();
   }

   public long maxWait() {
      return this.jniConnectionPoolConfiguration.MaxWait();
   }

   public int maxIdle() {
      return this.jniConnectionPoolConfiguration.MaxIdle();
   }

   public int minIdle() {
      return this.jniConnectionPoolConfiguration.MinIdle();
   }

   public int numTestsPerEvictionRun() {
      return this.jniConnectionPoolConfiguration.NumTestsPerEvictionRun();
   }

   public long timeBetweenEvictionRuns() {
      return this.jniConnectionPoolConfiguration.TimeBetweenEvictionRuns();
   }

   public long minEvictableIdleTime() {
      return this.jniConnectionPoolConfiguration.MinEvictableIdleTime();
   }

   public boolean testOnBorrow() {
      return this.jniConnectionPoolConfiguration.TestOnBorrow();
   }

   public boolean testOnReturn() {
      return this.jniConnectionPoolConfiguration.TestOnReturn();
   }

   public boolean testWhileIdle() {
      return this.jniConnectionPoolConfiguration.TestWhileIdle();
   }

   @Override
   public String toString() {
      return "ConnectionPoolConfiguration [exhaustedAction=" + exhaustedAction() + ", lifo=" + lifo() + ", maxActive=" + maxActive() + ", maxTotal=" + maxTotal() + ", maxWait=" + maxWait()
            + ", maxIdle=" + maxIdle() + ", minIdle=" + minIdle() + ", numTestsPerEvictionRun=" + numTestsPerEvictionRun() + ", timeBetweenEvictionRuns=" + timeBetweenEvictionRuns()
            + ", minEvictableIdleTime=" + minEvictableIdleTime() + ", testOnBorrow=" + testOnBorrow() + ", testOnReturn=" + testOnReturn() + ", testWhileIdle=" + testWhileIdle() + "]";
   }

}
