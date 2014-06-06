package org.infinispan.client.hotrod;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class ReflexVersionedValue {
   protected final Object self;
   private final Method getValueMethod;
   private final Method getVersionMethod;

   public ReflexVersionedValue(ClassLoader classLoader, Object self) throws ClassNotFoundException, NoSuchMethodException {
      this.self = self;
      Class<?> clazz = classLoader.loadClass("org.infinispan.client.hotrod.VersionedValue");
      getVersionMethod = clazz.getMethod("getVersion");
      getValueMethod = clazz.getMethod("getValue");
   }

   public long getVersion() throws InvocationTargetException, IllegalAccessException {
      return (Long) getVersionMethod.invoke(self);
   }

   public Object getValue() throws InvocationTargetException, IllegalAccessException {
      return getValueMethod.invoke(self);
   }
}
