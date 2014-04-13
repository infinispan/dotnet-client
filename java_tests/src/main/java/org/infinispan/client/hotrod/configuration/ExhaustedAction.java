package org.infinispan.client.hotrod.configuration;

/**
 * Enumeration for whenExhaustedAction. Order is important, as the underlying commons-pool uses a byte to represent values
 * ExhaustedAction.
 *
 * @author Tristan Tarrant
 * @since 5.3
 */
public enum ExhaustedAction {
    EXCEPTION, WAIT, CREATE_NEW
   // EXCEPTION(cli.Infinispan.HotRod.Config.ExhaustedAction.EXCEPTION), // GenericKeyedObjectPool.WHEN_EXHAUSTED_FAIL
   // WAIT(cli.Infinispan.HotRod.Config.ExhaustedAction.WAIT), // GenericKeyedObjectPool.WHEN_EXHAUSTED_BLOCK
   // CREATE_NEW(cli.Infinispan.HotRod.Config.ExhaustedAction.CREATE_NEW); // GenericKeyedObjectPool.WHEN_EXHAUSTED_GROW

   // private cli.Infinispan.HotRod.Config.ExhaustedAction jniExhaustedAction;
   
   // ExhaustedAction(cli.Infinispan.HotRod.Config.ExhaustedAction exhaustedAction){
   //    this.jniExhaustedAction = exhaustedAction;
   // }
   
   // public int getExhaustedActionInt() {
   //    return jniExhaustedAction;
   // }
   
   // public cli.Infinispan.HotRod.Config.ExhaustedAction getJniExhaustedAction() {
   //    return this.jniExhaustedAction;
   // }
   
   // public ExhaustedAction setExhaustedAction(cli.Infinispan.HotRod.Config.ExhaustedAction exhaustedAction){
   //    this.jniExhaustedAction = exhaustedAction;
   //    return this;
   // }
}
