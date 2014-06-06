package org.infinispan.client.hotrod;

import java.lang.reflect.Array;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.Arrays;
import java.util.Map;
import java.util.concurrent.TimeUnit;

public class ReflexCache {
   private final Object FORCE_RETURN_FLAGS;
   private final Object self;
   private final ClassLoader classLoader;
   private final Method sizeMethod;
   private final Method isEmptyMethod;
   private final Method putLIMethod;
   private final Method clearMethod;
   private final Method replaceMethod;
   private final Method replaceLIMethod;
   private final Method getBulkIMethod;
   private final Method removeMethod;
   private final Method putIfAbsentLIMethod;
   private final Method getVersionedMethod;
   private final Method replaceWithVersionMethod;
   private final Method removeWithVersionMethod;
   private Method putMethod;
   private Method getMethod;
   private Method containsKeyMethod;
   private Method getWithMetadataMethod;
   private Method statsMethod;
   private Method getStatsMapMethod;
   private Method withFlagsMethod;

   public ReflexCache(ClassLoader classLoader, Object self) throws ClassNotFoundException, NoSuchMethodException {
      this.self = self;
      this.classLoader = classLoader;

      Class<?> clazz = classLoader.loadClass("org.infinispan.client.hotrod.RemoteCache");
      putMethod = clazz.getMethod("put", new Class[] { Object.class, Object.class });
      putLIMethod = clazz.getMethod("put", new Class[] { Object.class, Object.class, long.class, TimeUnit.class, long.class, TimeUnit.class });
      putIfAbsentLIMethod = clazz.getMethod("putIfAbsent", new Class[] { Object.class, Object.class, long.class, TimeUnit.class, long.class, TimeUnit.class });
      replaceMethod = clazz.getMethod("replace", new Class[] { Object.class, Object.class });
      replaceLIMethod = clazz.getMethod("replace", new Class[] { Object.class, Object.class, long.class, TimeUnit.class, long.class, TimeUnit.class });
      replaceWithVersionMethod = clazz.getMethod("replaceWithVersion", new Class[] { Object.class, Object.class, long.class, int.class, int.class });
      getMethod = clazz.getMethod("get", new Class[] { Object.class });
      removeMethod = clazz.getMethod("remove", new Class[] { Object.class });
      removeWithVersionMethod = clazz.getMethod("removeWithVersion", new Class[] { Object.class, long.class });
      containsKeyMethod = clazz.getMethod("containsKey", new Class[] { Object.class });
      getWithMetadataMethod = clazz.getMethod("getWithMetadata", new Class[] { Object.class });
      getVersionedMethod = clazz.getMethod("getVersioned", new Class[] { Object.class });
      getBulkIMethod = clazz.getMethod("getBulk", new Class[] { int.class });
      statsMethod = clazz.getMethod("stats", new Class[] {});
      sizeMethod = clazz.getMethod("size", new Class[] {});
      isEmptyMethod = clazz.getMethod("isEmpty", new Class[] {});
      clearMethod = clazz.getMethod("clear", new Class[] {});

      Class<?> flagClazz = classLoader.loadClass("org.infinispan.client.hotrod.Flag");
      FORCE_RETURN_FLAGS = Array.newInstance(flagClazz, 1);
      Array.set(FORCE_RETURN_FLAGS, 0, Enum.valueOf((Class<? extends Enum>) flagClazz, "FORCE_RETURN_VALUE"));
      withFlagsMethod = clazz.getMethod("withFlags", FORCE_RETURN_FLAGS.getClass());

      Class<?> javaSSClass = classLoader.loadClass("org.infinispan.client.hotrod.ServerStatistics");
      getStatsMapMethod = javaSSClass.getMethod("getStatsMap", new Class[] {});
   }

   public boolean isEmpty() throws InvocationTargetException, IllegalAccessException {
      return (Boolean) isEmptyMethod.invoke(self);
   }


   public int size() throws InvocationTargetException, IllegalAccessException {
      return (Integer) sizeMethod.invoke(self);
   }

   public Object put(Object key, Object value) throws InvocationTargetException, IllegalAccessException {
      return putMethod.invoke(self, key, value);
   }

   public Object put(Object key, Object value, boolean forceReturnValue, long lifespan, long idle) throws InvocationTargetException, IllegalAccessException {
      Object me = forceReturnValue ? withFlagsMethod.invoke(self, FORCE_RETURN_FLAGS) : self;
      return putLIMethod.invoke(me, key, value, lifespan, TimeUnit.SECONDS, idle, TimeUnit.SECONDS);
   }

   public Object get(Object key) throws InvocationTargetException, IllegalAccessException {
      return getMethod.invoke(self, key);
   }

   public void clear() throws InvocationTargetException, IllegalAccessException {
      clearMethod.invoke(self);
   }

   public Object replace(Object key, Object value, boolean forceReturnValue) throws InvocationTargetException, IllegalAccessException {
      Object me = forceReturnValue ? withFlagsMethod.invoke(self, FORCE_RETURN_FLAGS) : self;
      return replaceMethod.invoke(me, key, value);
   }

   public Object replace(String key, Object value, boolean forceReturnValue, long lifespan, long idle) throws InvocationTargetException, IllegalAccessException {
      Object me = forceReturnValue ? withFlagsMethod.invoke(self, FORCE_RETURN_FLAGS) : self;
      return replaceLIMethod.invoke(me, key, value, lifespan, TimeUnit.SECONDS, idle, TimeUnit.SECONDS);
   }

   public Map<Object, Object> getBulk(int numEntries) throws InvocationTargetException, IllegalAccessException {
      return (Map) getBulkIMethod.invoke(self, numEntries);
   }

   public Object remove(Object key, boolean forceReturnValue) throws InvocationTargetException, IllegalAccessException {
      Object me = forceReturnValue ? withFlagsMethod.invoke(self, FORCE_RETURN_FLAGS) : self;
      return removeMethod.invoke(me, key);
   }

   public boolean containsKey(Object key) throws InvocationTargetException, IllegalAccessException {
      return (Boolean) containsKeyMethod.invoke(self, key);
   }

   public Object putIfAbsent(Object key, Object value, boolean forceReturnValue, long lifespan, long idle) throws InvocationTargetException, IllegalAccessException {
      Object me = forceReturnValue ? withFlagsMethod.invoke(self, FORCE_RETURN_FLAGS) : self;
      return putIfAbsentLIMethod.invoke(me, key, value, lifespan, TimeUnit.SECONDS, idle, TimeUnit.SECONDS);
   }

   public ReflexMetadataValue getWithMetadata(Object key) throws InvocationTargetException, IllegalAccessException, NoSuchMethodException, ClassNotFoundException {
      return new ReflexMetadataValue(classLoader, getWithMetadataMethod.invoke(self, key));
   }

   public ReflexVersionedValue getVersioned(Object key) throws InvocationTargetException, IllegalAccessException, NoSuchMethodException, ClassNotFoundException {
      return new ReflexVersionedValue(classLoader, getVersionedMethod.invoke(self, key));
   }

   public boolean replaceWithVersion(Object key, long currentVersion, Object value, long lifespan, long idle) throws InvocationTargetException, IllegalAccessException {
      return (Boolean) replaceWithVersionMethod.invoke(self, key, value, currentVersion, (int) lifespan, (int) idle);
   }

   public Map<String, String> getStats() throws InvocationTargetException, IllegalAccessException {
      Object stats = statsMethod.invoke(self);
      return (Map) getStatsMapMethod.invoke(stats);
   }

   public boolean removeWithVersion(Object key, long currentVersion) throws InvocationTargetException, IllegalAccessException {
      return (Boolean) removeWithVersionMethod.invoke(self, key, currentVersion);
   }
}
