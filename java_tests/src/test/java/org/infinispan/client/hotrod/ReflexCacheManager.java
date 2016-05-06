package org.infinispan.client.hotrod;

import org.infinispan.client.hotrod.logging.Log;
import org.infinispan.client.hotrod.logging.LogFactory;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import static org.testng.Assert.fail;

public class ReflexCacheManager {
   private static final Log log = LogFactory.getLog(ReflexCacheManager.class);
   private ClassLoader classLoader;
   private Object self;
   private Method getCacheMethod;
   private Method stopMethod;
   private Method isStarted;
   private Method javaAddServersMethod;
   private Method javaConfBuildMethod;
    
   public ReflexCacheManager(ClassLoader classLoader, String serverList) throws ClassNotFoundException, NoSuchMethodException, IllegalAccessException, InvocationTargetException, InstantiationException {
      this(classLoader, serverList, false);
   } 
   public ReflexCacheManager(ClassLoader classLoader, String serverList, boolean useCompatibilityStringMarshaller) throws ClassNotFoundException, NoSuchMethodException, IllegalAccessException, InvocationTargetException, InstantiationException {
      this.classLoader = classLoader;

      Class<?> clazz = classLoader.loadClass("org.infinispan.client.hotrod.RemoteCacheManager");
      Class javaConfClass = classLoader.loadClass("org.infinispan.client.hotrod.configuration.Configuration");
      Class javaConfBuildClass = classLoader.loadClass("org.infinispan.client.hotrod.configuration.ConfigurationBuilder");
      log.infof("Loaded RemoteCacheManager class from %s, resource is %s",
            clazz.getProtectionDomain().getCodeSource().getLocation(),
            clazz.getResource('/' + clazz.getName().replace('.', '/') + ".class"));
       try {
          clazz.getMethod("getJniManager");
          fail("Could not load Java Hot Rod Client RemoteCacheManager");
       } catch (Exception e) {
          // This should throw an exception
       }

      final Boolean startServer = true;
      Constructor<?> ctor = null;
      if (useCompatibilityStringMarshaller) {
         ctor = clazz.getConstructor(new Class[] { String.class, boolean.class, boolean.class });
         self = ctor.newInstance(serverList, startServer, useCompatibilityStringMarshaller);
      } else {
         //the original HotRod client doesn't support the third argument 
         ctor = clazz.getConstructor(new Class[] { javaConfClass });
         javaAddServersMethod= javaConfBuildClass.getDeclaredMethod("addServers", new Class[] { String.class });
         javaConfBuildMethod = javaConfBuildClass.getDeclaredMethod("build", null);
         Object newInstance = javaConfBuildClass.getConstructor(null).newInstance();
         Object invoke = javaAddServersMethod.invoke(newInstance, serverList);
         self = ctor.newInstance(javaConfBuildMethod.invoke(invoke));
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
