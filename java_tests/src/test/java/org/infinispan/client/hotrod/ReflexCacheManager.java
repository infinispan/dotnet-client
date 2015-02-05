package org.infinispan.client.hotrod;

import org.infinispan.client.hotrod.logging.Log;
import org.infinispan.client.hotrod.logging.LogFactory;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class ReflexCacheManager {
   private static final Log log = LogFactory.getLog(ReflexCacheManager.class);
   private ClassLoader classLoader;
   private Object self;
   private Method getCacheMethod;
   private Method stopMethod;
   private Method isStarted;
    
   public ReflexCacheManager(ClassLoader classLoader, String serverList) throws ClassNotFoundException, NoSuchMethodException, IllegalAccessException, InvocationTargetException, InstantiationException {
      this(classLoader, serverList, false);
   } 
   public ReflexCacheManager(ClassLoader classLoader, String serverList, boolean useCompatibilityStringMarshaller) throws ClassNotFoundException, NoSuchMethodException, IllegalAccessException, InvocationTargetException, InstantiationException {
      this.classLoader = classLoader;

      Class<?> clazz = classLoader.loadClass("org.infinispan.client.hotrod.RemoteCacheManager");
      log.infof("Loaded RemoteCacheManager class from %s, resource is %s",
            clazz.getProtectionDomain().getCodeSource().getLocation(),
            clazz.getResource('/' + clazz.getName().replace('.', '/') + ".class"));

      final Boolean startServer = true;
      Constructor<?> ctor = null;
      if (useCompatibilityStringMarshaller) {
         ctor = clazz.getConstructor(new Class[] { String.class, boolean.class, boolean.class });
         self = ctor.newInstance(serverList, startServer, useCompatibilityStringMarshaller);
      } else {
         //the original HotRod client doesn't support the third argument 
         ctor = clazz.getConstructor(new Class[] { String.class, boolean.class });
         self = ctor.newInstance(serverList, startServer);
      }
      
      getCacheMethod = clazz.getDeclaredMethod("getCache", new Class[] { String.class });
      stopMethod = clazz.getDeclaredMethod("stop", new Class[] {});
      isStarted = clazz.getDeclaredMethod("isStarted", new Class[] {});
      
      
   }

   public ReflexCache getCache(String name) throws InvocationTargetException, IllegalAccessException, NoSuchMethodException, ClassNotFoundException {
      return new ReflexCache(classLoader, getCacheMethod.invoke(self, name));
   }

   public void stop() throws InvocationTargetException, IllegalAccessException {
      stopMethod.invoke(self);
   }

   public boolean isStarted() throws InvocationTargetException, IllegalAccessException {
      return (Boolean) isStarted.invoke(self);
   }
}
