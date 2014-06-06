package org.infinispan.client.hotrod;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class ReflexMetadataValue extends ReflexVersionedValue {
   private Method getCreatedMethod;
   private Method getLastUsedMethod;
   private Method getLifespanMethod;
   private Method getMaxIdleMethod;

   public ReflexMetadataValue(ClassLoader classLoader, Object self) throws ClassNotFoundException, NoSuchMethodException {
      super(classLoader, self);
      Class<?> clazz = classLoader.loadClass("org.infinispan.client.hotrod.MetadataValue");
      getCreatedMethod = clazz.getMethod("getCreated");
      getLastUsedMethod = clazz.getMethod("getLastUsed");
      getLifespanMethod = clazz.getMethod("getLifespan");
      getMaxIdleMethod = clazz.getMethod("getMaxIdle");
   }

   public int getLifespan() throws InvocationTargetException, IllegalAccessException {
      return (Integer) getLifespanMethod.invoke(self);
   }

   public int getMaxIdle() throws InvocationTargetException, IllegalAccessException {
      return (Integer) getMaxIdleMethod.invoke(self);
   }

   public long getCreated() throws InvocationTargetException, IllegalAccessException {
      return (Long) getCreatedMethod.invoke(self);
   }

   public long getLastUsed() throws InvocationTargetException, IllegalAccessException {
      return (Long) getLastUsedMethod.invoke(self);
   }
}
